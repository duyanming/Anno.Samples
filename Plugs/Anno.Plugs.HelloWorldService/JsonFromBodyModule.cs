using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anno.Plugs.HelloWorldService
{
    using Anno.EngineData;
    using Dto;
    public class JsonFromBodyModule : BaseModule
    {
        public JsonFromBodyModule()
        {

        }

        public Task<string> BodyString(string body)
        {
            return Task.FromResult($"body from anno service {body}");
        }

        public Task<JsonFromBodyObjDto> BodyObj(JsonFromBodyObjDto body)
        {
            body.Name += "-Anno";
            return Task.FromResult(body);
        }
    }
}
