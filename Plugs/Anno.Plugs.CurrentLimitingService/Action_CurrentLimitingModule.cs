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

    public class Action_CurrentLimitingModule : BaseModule
    {
        public Action_CurrentLimitingModule()
        {

        }
        /*
         *  参数1：限流算法是令牌桶还是漏桶
         *  参数2：限流时间片段单位秒
         *  参数3：单位时间可以通过的请求个数
         *  参数4：桶容量
         */
        [AnnoInfo(Desc = "接口限流(令牌容量3 每秒通过1个) LimitingType.TokenBucket, 1, 1, 3")]
        [RateLimit(LimitingType.TokenBucket, 1, 1, 3)]
        public Task<string> SayHi_TokenBucket(string name)
        {
            return Task.FromResult($"{name} 你好，我是Anno.");
        }
        [AnnoInfo(Desc = "未限流")]
        public Task<string> GoodBye(string name)
        {
            return Task.FromResult($"{name} 再见，有缘再会.");
        }
    }
}
