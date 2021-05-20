using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamplesWebApi.Controllers
{
    using Anno.Rpc.Client;

    [Route("api/[Controller]/[action]")]
    public class NoInterfaceCallAnnoController : ControllerBase
    {
        public NoInterfaceCallAnnoController()
        {

        }
        /// <summary>
        /// 说问候，你好啊 SayHi
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>

        [HttpGet]
        [HttpPost]
        public string SayHi(string name)
        {
            Dictionary<string, string> input = new Dictionary<string, string>();
            input.Add(Anno.Const.Enum.Eng.NAMESPACE, "Anno.Plugs.HelloWorld");
            input.Add(Anno.Const.Enum.Eng.CLASS, "HelloAnno");
            input.Add(Anno.Const.Enum.Eng.METHOD, "SayHi");
            input.Add("name", name);
            return Connector.BrokerDns(input);
        }
        /// <summary>
        /// 异步 说问候，你好啊 TaskSayHi
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public async Task<string> TaskSayHi(string name)
        {
            Dictionary<string, string> input = new Dictionary<string, string>();
            input.Add(Anno.Const.Enum.Eng.NAMESPACE, "Anno.Plugs.HelloWorld");
            input.Add(Anno.Const.Enum.Eng.CLASS, "HelloAnno");
            input.Add(Anno.Const.Enum.Eng.METHOD, "SayHi");
            input.Add("name", name);
            return await Connector.BrokerDnsAsync(input);
        }
        /// <summary>
        /// 说再见 GoodBye
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public string GoodBye(string name)
        {
            Dictionary<string, string> input = new Dictionary<string, string>();
            input.Add(Anno.Const.Enum.Eng.NAMESPACE, "Anno.Plugs.HelloWorld");
            input.Add(Anno.Const.Enum.Eng.CLASS, "HelloAnno");
            input.Add(Anno.Const.Enum.Eng.METHOD, "GoodBye");
            input.Add("name", name);
            return Connector.BrokerDns(input);
        }
    }
}
