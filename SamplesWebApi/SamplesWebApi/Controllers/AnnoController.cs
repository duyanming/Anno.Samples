using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SamplesWebApi.Contract;

namespace SamplesWebApi.Controllers
{
    [Route("api/[Controller]/[action]")]
    public class AnnoController : ControllerBase
    {

        private readonly ILogger<AnnoController> _logger;
        private readonly AnnoConfig _annoConfig;

        private readonly IHelloAnnoService _helloAnnoService;

        private readonly INonstandardService _nonstandardService;

        public AnnoController(ILogger<AnnoController> logger, AnnoConfig annoConfig, IHelloAnnoService helloAnnoService, INonstandardService nonstandardService)
        {
            _logger = logger;
            _annoConfig = annoConfig;

            _helloAnnoService = helloAnnoService;
            _nonstandardService = nonstandardService;
        }
        [HttpGet]
        public dynamic Index()
        {

            return Content(
                $"欢迎使用Anno微服务框架" +
                   $"<br/>Anno应用名称：{_annoConfig.AppName}" +
                   $"<br/>Anno注册中心地址：{_annoConfig.IpAddress}" +
                   $"<br/>Anno注册中心端口：{_annoConfig.Port}" +
                   $"<br/>Anno链路追踪是否开启：{_annoConfig.TraceOnOff}"
                );
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
            return _helloAnnoService.SayHi(name);
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
            return await _helloAnnoService.TaskSayHi(name);
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
            return _helloAnnoService.GoodBye(name);
        }

        /// <summary>
        /// 说再见 别名 GoodByeAlias
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public string GoodByeAlias(string name)
        {
            return _helloAnnoService.GoodByeAlias(name);
        }

        /// <summary>
        /// NonstandardServiceSayHi
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        public string NonstandardServiceSayHi(string name)
        {
            return _nonstandardService.SayHi(name);
        }
    }
}
