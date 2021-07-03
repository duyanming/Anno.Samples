using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Anno.Rpc.Client;
using Anno.Rpc.Client.DynamicProxy;
using ConsoleClient.Contract;

namespace ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "模拟控制台客户端";
            Init();

            //await ProxyTest();
            FromBodyTest();

            await Task.CompletedTask;
            Console.WriteLine("调用完成");
            Console.ReadLine();
        }
        /// <summary>
        /// 指定注册中心
        /// 连接池配置
        /// </summary>
        static void Init()
        {
            DefaultConfigManager.SetDefaultConnectionPool(100, Environment.ProcessorCount * 2, 20);
            DefaultConfigManager.SetDefaultConfiguration("ConsoleClient", "127.0.0.1", 7010, false);
        }

        static async Task ProxyTest()
        {
            var helloAnnoService = AnnoProxyBuilder.GetService<IHelloAnnoService>();
            var sayHi = helloAnnoService.SayHi($"jack ma--[SayHi]");
            Console.WriteLine(sayHi);


            var taskSayHi = await helloAnnoService.TaskSayHi($"jack ma--[TaskSayHi]");
            Console.WriteLine(taskSayHi);

            var goodBye = helloAnnoService.GoodBye($"jack ma--[GoodBye]");
            Console.WriteLine(goodBye);

            var goodByeAlias = helloAnnoService.GoodByeAlias($"jack ma--[GoodByeAlias]");
            Console.WriteLine(goodByeAlias);
        }

        static void FromBodyTest()
        {
            List<string> list = new List<string>();
            list.Add("test");
            list.Add("test2");
            Dictionary<string, string> input = new Dictionary<string, string>();
            input["token"] = "test";
            input["permissions"] = Newtonsoft.Json.JsonConvert.SerializeObject(list);

            input[Anno.Const.Enum.Eng.NAMESPACE] = "Anno.Plugs.HelloWorld";
            input[Anno.Const.Enum.Eng.CLASS] = "HelloAnno";
            input[Anno.Const.Enum.Eng.METHOD] = "Validate";
            //var str = Newtonsoft.Json.JsonConvert.SerializeObject(input);
            //Console.WriteLine(str);
            //var listStr = Newtonsoft.Json.JsonConvert.SerializeObject(list);
            //Console.WriteLine(listStr);
            //var x = Newtonsoft.Json.JsonConvert.DeserializeObject(listStr, typeof(List<string>));
            //var x1 = ToObjFromDic(typeof(ValidateReq), input);

            var rlt = Connector.BrokerDns(input);
            Console.WriteLine(rlt);
        }
        private static object ToObjFromDic(Type type, Dictionary<string, string> input)
        {
            var body = type.Assembly.CreateInstance(type.FullName);
            List<PropertyInfo> targetProps = type.GetProperties().Where(p => p.CanWrite == true).ToList();
            var fields = type.GetFields().Where(p => p.IsPublic).ToList();
            if (targetProps != null && targetProps.Count > 0)
            {
                var keys = input.Keys.ToList();
                foreach (var propertyInfo in targetProps)
                {
                    foreach (var key in keys)
                    {
                        if (key.Equals(propertyInfo.Name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            var valueStr = input[key];
                            try
                            {
                                if (propertyInfo.PropertyType.IsPrimitive || 
                                    (propertyInfo.PropertyType.FullName.StartsWith("System.")&&!propertyInfo.PropertyType.FullName.StartsWith("System.Collections.Generic")))
                                {
                                    var value = Convert.ChangeType(valueStr, propertyInfo.PropertyType);
                                    propertyInfo.SetValue(body, value, null);
                                }
                                else if (propertyInfo.PropertyType.BaseType == typeof(Enum))
                                {
                                    var value = Enum.Parse(propertyInfo.PropertyType, valueStr);
                                    propertyInfo.SetValue(body, value, null);
                                }
                                else
                                {
                                    var value = Newtonsoft.Json.JsonConvert.DeserializeObject(valueStr, propertyInfo.PropertyType);
                                    propertyInfo.SetValue(body, value, null);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                            break;
                        }
                    }
                }
            }
            return body;
        }
    }
    public class ValidateReq
    {
        public List<string> permissions { get; set; }
        public string token { get; set; }
    }
}
