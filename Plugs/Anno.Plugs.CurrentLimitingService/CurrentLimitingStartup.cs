using Anno.EngineData;
using System;

namespace Anno.Plugs.CurrentLimitingService
{
    using Anno.RateLimit;
    using Anno.EngineData;
    using Anno.EngineData.Limit;

    public class CurrentLimitingStartup : IPlugsConfigurationBootstrap
    {
        /// <summary>
        /// Ioc 容器构建后
        /// </summary>
        public void ConfigurationBootstrap()
        { 
            //全局限流
            //var globalRateLimit = new RateLimit(LimitingType.TokenBucket, 1, 3, 3);
            //EngineData.Routing.Routing.AddFilter(globalRateLimit);
        }
        /// <summary>
        /// Ioc 容器构建前
        /// </summary>
        public void PreConfigurationBootstrap()
        {

        }
    }
}
