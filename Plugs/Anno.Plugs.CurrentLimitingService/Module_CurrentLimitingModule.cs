using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anno.Plugs.CurrentLimitingService
{
    using Anno.RateLimit;
    using Anno.EngineData;
    using Anno.EngineData.Limit;
    using Anno.Const.Attribute;
    /*
 *  参数1：限流算法是令牌桶还是漏桶
 *  参数2：限流时间片段单位秒
 *  参数3：单位时间可以通过的请求个数
 *  参数4：桶容量limitsize 是应对突发流量的
 *  Module 和Action上都有RateLimit的时候都会生效，谁单位时间限制请求书低谁先被触发
 */
    [RateLimit(LimitingType.TokenBucket, 1, 2, 3)]
    public class Module_CurrentLimitingModule : BaseModule
    {
        public Module_CurrentLimitingModule()
        {

        }
        [AnnoInfo(Desc = "Module接口限流")]
        public Task<string> SayHi(string name)
        {
            return Task.FromResult($"{name} 你好，我是Anno.");
        }
        [AnnoInfo(Desc = "Module接口限流")]
        public Task<string> GoodBye(string name)
        {
            return Task.FromResult($"{name} 再见，有缘再会.");
        }
    }
}
