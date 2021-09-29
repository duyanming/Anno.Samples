using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore
{
    using Anno.Const.Enum;
    using Anno.EngineData;
    using Anno.Rpc.Client;

    /// <summary>
    /// 接入服务中心的HostBuilder中间件
    /// </summary>
    public static class AnnoWebHostBuilderExtensions
    {
        public static IWebHostBuilder UseAnnoSvc(this IWebHostBuilder hostBuilder)
        {
            if (hostBuilder == null)
            {
                throw new ArgumentNullException(nameof(hostBuilder));
            }
            // 检查是否已经加载过了
            if (hostBuilder.GetSetting(nameof(UseAnnoSvc)) != null)
            {
                return hostBuilder;
            }
            // 设置已加载标记，防止重复加载
            hostBuilder.UseSetting(nameof(UseAnnoSvc), true.ToString());
            // 添加configure处理
            hostBuilder.ConfigureServices(services =>
            {
                #region 注册路由和限流
                services.AddSingleton<ISelectRoute, UserCodeSelectRoute>();
                services.AddSingleton<IRateLimitStrategy, UserCodeRateLimitStrategy>(); 
                #endregion

                var serviceProvider = services.BuildServiceProvider();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                IRateLimitStrategy rateLimitStrategy = serviceProvider.GetService<IRateLimitStrategy>();
                ISelectRoute selectRoute = serviceProvider.GetRequiredService<ISelectRoute>();

                AnnoGatewayConfig gatewayConfig = new AnnoGatewayConfig();
                configuration.Bind(gatewayConfig);
                services.AddSingleton(gatewayConfig);
                services.AddSingleton<IStartupFilter>(new AnnoSetupFilter(gatewayConfig,rateLimitStrategy,selectRoute));
            });


            return hostBuilder;
        }
    }
    class AnnoSetupFilter : IStartupFilter
    {
        private readonly AnnoGatewayConfig gatewayConfig = new AnnoGatewayConfig();
        private readonly IRateLimitStrategy rateLimitStrategy;
        private readonly ISelectRoute selectRoute;
        public AnnoSetupFilter(AnnoGatewayConfig _gatewayConfig, IRateLimitStrategy rateLimitStrategy, ISelectRoute selectRoute)
        {
            this.gatewayConfig = _gatewayConfig;
            this.rateLimitStrategy = rateLimitStrategy;
            this.selectRoute = selectRoute;
        }
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseRouting();
                app.UseMiddleware<AnnoMiddleware>();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.Map("AnnoApi/{channel}/{router}/{method}", AnnoApi);
                });
                next(app);
            };
        }
        private Task AnnoApi(HttpContext context)
        {
            var routeValues = context.Request.RouteValues;
            routeValues.TryGetValue("channel", out object channel);
            routeValues.TryGetValue("router", out object router);
            routeValues.TryGetValue("method", out object method);
            routeValues.TryGetValue("nodeName", out object nodeName);

            return ApiInvoke(context, (input) =>
            {
                input[Eng.NAMESPACE] = channel.ToString();
                input[Eng.CLASS] = router.ToString();
                input[Eng.METHOD] = method.ToString();

                string nickName = selectRoute.SelectRoute(context);
                return Connector.BrokerDnsAsync(input, nickName).ConfigureAwait(false).GetAwaiter().GetResult();
            });
        }

        private Task ApiInvoke(HttpContext context, Func<Dictionary<string, string>, string> invoke)
        {
            context.Response.ContentType = "Content-Type: application/javascript; charset=utf-8";
            Dictionary<string, string> input = new Dictionary<string, string>();
            #region 接收表单参数
            var Request = context.Request;
            var headers = Request.Headers;
            try
            {
                if (headers != null && headers.ContainsKey("X-Original-For") && !headers["X-Original-For"].ToArray()[0].StartsWith("127.0.0.1"))
                {
                    input.Add("X-Original-For", headers["X-Original-For"].ToArray()[0]);
                }
                else
                {
                    input.Add("X-Original-For", $"{context.Connection.RemoteIpAddress.ToString().Replace("::ffff:", "")}:{context.Connection.RemotePort}");
                }
            }
            finally
            {
                try
                {
                    if (Request.Method == "POST")
                    {
                        foreach (string k in Request.Form.Keys)
                        {
                            input.Add(k, Request.Form[k]);
                        }
                        foreach (IFormFile file in Request.Form.Files)
                        {
                            var fileName = file.Name;
                            if (string.IsNullOrWhiteSpace(fileName))
                            {
                                fileName = file.FileName;
                            }
                            input.TryAdd(fileName, file.ToBase64());
                        }
                    }
                }
                finally
                {
                    foreach (string k in Request.Query.Keys)
                    {
                        if (!input.ContainsKey(k))
                        {
                            input.Add(k, Request.Query[k]);
                        }
                    }
                }
            }
            #endregion
            #region 限流
            if (rateLimitStrategy!=null&&!rateLimitStrategy.Request(context))
            {
                context.Response.StatusCode = 429;
                Dictionary<string, object> rlt = new Dictionary<string, object>();
                rlt.Add("status", false);
                rlt.Add("msg", "Trigger current limiting.");
                rlt.Add("output", null);
                rlt.Add("outputData", 429);
                var rltExec = System.Text.Encoding.UTF8.GetString(rlt.ExecuteResult());
                input.TryAdd("TraceId", Guid.NewGuid().ToString());
                input.TryAdd("GlobalTraceId", Guid.NewGuid().ToString());
                input.TryAdd("AppName", gatewayConfig.AppName);
                input.TryAdd("AppNameTarget", gatewayConfig.AppName);
                TracePool.EnQueue(TracePool.CreateTrance(input), FailMessage("Trigger current limiting.", false));
                return context.Response.WriteAsync(rltExec);
            }
            #endregion
            #region 处理
            ActionResult actionResult = null;
            try
            {
                //分发器
                var rlt = invoke(input);
                actionResult = JsonConvert.DeserializeObject<ActionResult>(rlt);

            }
            catch (Exception ex)
            {
                actionResult = new ActionResult()
                {
                    Msg = ex.Message,
                    Status = true
                };
            }
            #region 将OutPut # 开头的键 推到返回数据的根目录
            Dictionary<string, object> rltd = new Dictionary<string, object>();
            List<string> keys = new List<string>();
            rltd.Add("msg", actionResult.Msg);
            rltd.Add("status", actionResult.Status);
            if (actionResult.Output != null)
            {
                keys.AddRange(actionResult.Output.Keys);
                keys = keys.FindAll(k => k.Substring(0, 1) == "#");
                foreach (string key in keys)
                {
                    string newKey = key.Substring(1);
                    rltd.Add(newKey, actionResult.Output[key]);
                    actionResult.Output.Remove(key);
                }
                rltd.Add("output", actionResult.Output);
            }
            else
            {
                rltd.Add("output", null);
            }
            rltd.Add("outputData", actionResult.OutputData);
            #endregion
            #endregion
            return context.Response.WriteAsync(System.Text.Encoding.UTF8.GetString(rltd.ExecuteResult()));
        }

        #region 工具

        /// <summary>
        /// 构建错误消息Json字符串
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="status">默认False</param>
        /// <returns>"{\"Msg\":\""+message+"\",\"Status\":false,\"Output\":null,\"OutputData\":null}"</returns>
        internal string FailMessage(string message, bool status = false)
        {
            return "{\"Msg\":\"" + message + "\",\"Status\":" + status.ToString().ToLower() +
                   ",\"Output\":null,\"OutputData\":null}";
        }
        #endregion 
    }
    class AnnoMiddleware
    {
        private readonly RequestDelegate _next;
        private AnnoGatewayConfig gatewayConfig = new AnnoGatewayConfig();
        public AnnoMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            configuration.Bind(gatewayConfig);
            DefaultConfigManager
                .SetDefaultConfiguration(gatewayConfig.AppName
                , gatewayConfig.IpAddress
                , gatewayConfig.Port
                , gatewayConfig.TraceOnOff);
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var headers = httpContext.Response.Headers;

            try
            {
                //解析访问者IP地址和端口号
                if (headers != null)
                {
                    headers.TryAdd("Server", "Anno/1.0");
                }
            }
            finally
            {
                await _next(httpContext);
            }

        }
    }
    #region 扩展Newtonsoft.Json 序列化
    public class BigIntJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(long) || objectType == typeof(long?))
            {
                return true;
            }

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (null != existingValue)
            {
                return long.Parse(existingValue.ToString());
            }
            else
            {
                return DBNull.Value;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteValue(value.ToString());
        }
    }

    public static class DicExt
    {
        public static byte[] ExecuteResult(this Dictionary<string, object> dic)
        {
            if (dic == null)
            {
                return default(byte[]);
            }
            MemoryStream ms = new MemoryStream();
            StreamWriter writer = new StreamWriter(ms);
            JsonSerializer jsonSerializer = new JsonSerializer();
            jsonSerializer.Converters.Add(new BigIntJsonConverter());

            jsonSerializer.Serialize(writer, dic);
            writer.Flush();
            writer.Close();
            return ms.ToArray();
        }
    }
    #endregion
}
