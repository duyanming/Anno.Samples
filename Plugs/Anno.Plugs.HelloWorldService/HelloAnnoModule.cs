using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anno.Plugs.HelloWorldService
{
    using Anno.EngineData;
    public class HelloAnnoModule:BaseModule
    {
        public HelloAnnoModule()
        {
            
        }

        public Task<string> SayHi(string name)
        {

            return Task.FromResult($"{name} 你好，我是Anno.");
        }
    }
}
