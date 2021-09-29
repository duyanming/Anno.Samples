using Anno.CronNET;
using Anno.RateLimit;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;

namespace Microsoft.AspNetCore
{
    public class UserCodeRateLimitStrategy : IRateLimitStrategy
    {
        private volatile ConcurrentDictionary<string, LimitInfo> _rateLimitPool = new ConcurrentDictionary<string, LimitInfo>();
        private readonly CronDaemon CronDaemon = new CronDaemon();
        /// <summary>
        /// true 代表不限制
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Request(HttpContext context)
        {
            bool notLimit = true;
            var Request = context.Request;
            //var headers = Request.Headers;
            string userCode = Request.Query["userCode"];

            _rateLimitPool.TryGetValue(userCode, out LimitInfo limitInfo);
            if (limitInfo == null)
            {
                var limitCfg = selectLimitCfg(userCode);
                var limitingService = LimitingFactory.Build(TimeSpan.FromSeconds(limitCfg.timeSpan)
                         , LimitingType.LeakageBucket
                         , (int)limitCfg.maxQPS
                         , (int)limitCfg.limitSize);
                limitInfo = new LimitInfo()
                {
                    Time = DateTime.Now,
                    limitingService = limitingService
                };
                _rateLimitPool.TryAdd(userCode, limitInfo);

            }
            if (limitInfo != null)
            {
                //limit.Request() ==true 代表不受限制
                limitInfo.Time = DateTime.Now;
                notLimit = limitInfo.limitingService.Request();
            }
            if (!notLimit)
            {
#if DEBUG
                Console.WriteLine($"userCode:{userCode},Trigger current limiting.");
#endif
            }

            return notLimit;
        }
        /// <summary>
        /// 张三 rps  10  其他人2， 用户根据需要自定义
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        private LimitCfg selectLimitCfg(string userCode)
        {
            LimitCfg limitCfg = new LimitCfg();
            if (userCode.Equals("zhangsan"))
            {
                limitCfg.timeSpan = 1;
                limitCfg.limitSize = 10;
                limitCfg.maxQPS = 10;
            }
            else
            {
                limitCfg.timeSpan = 1;
                limitCfg.limitSize = 2;
                limitCfg.maxQPS = 2;
            }
            return limitCfg;
        }
    }
    /// <summary>
    /// 限流 配置
    /// </summary>
    public class LimitCfg
    {
        /// <summary>
        /// 时间片段  1
        /// </summary>
        public uint timeSpan { get; set; } = 1;
        /// <summary>
        /// 单位 时间片段 内允许通过的请求个数  2
        /// </summary>
        public uint maxQPS { get; set; } = 2;
        /// <summary>
        /// 令牌桶大小  2
        /// </summary>
        public uint limitSize { get; set; } = 2;
    }

    class LimitInfo
    {
        public DateTime Time { get; set; } = DateTime.Now;
        public ILimitingService limitingService { get; set; }
    }
}
