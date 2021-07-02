using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anno.Plugs.HelloWorldService
{
    using Anno.EngineData;
    using Dto;
    public class HelloAnnoModule : BaseModule
    {
        public HelloAnnoModule()
        {

        }

        public Task<string> SayHi(string name)
        {
            return Task.FromResult($"{name} 你好，我是Anno.");
        }

        public Task<string> GoodBye(string name)
        {
            return Task.FromResult($"{name} 再见，有缘再会.");
        }

        public ValidateReq Validate([FromBody] ValidateReq req)
        {
            if (req == null)
            {
                req = new ValidateReq();
            }
            var validRlt = req.IsValid();
            if (!validRlt.IsVaild) {
                req.token = Newtonsoft.Json.JsonConvert.SerializeObject(validRlt.ErrorMembers);
            }
            req.token = $"Hello {req.token ?? "FromBody"}";
            return req;
        }
    }
}
