using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SamplesWebApi.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamplesWebApi.Controllers
{
    [Route("api/[Controller]/[action]")]
    [ApiController]
    public class CurrentLimitingController : ControllerBase, ICurrentLimitingService
    {
        private readonly ICurrentLimitingService _currentLimitingService;

        public CurrentLimitingController(ICurrentLimitingService currentLimitingService)
        {
            _currentLimitingService = currentLimitingService;
        }
        [HttpGet]
        public string GoodBye_Action(string name)
        {
            return _currentLimitingService.GoodBye_Action(name);
        }
        [HttpGet]
        public string GoodBye_Global(string name)
        {
            return _currentLimitingService.GoodBye_Global(name);
        }
        [HttpGet]
        public string GoodBye_Module(string name)
        {
            return _currentLimitingService.GoodBye_Module(name);
        }
        [HttpGet]
        public string SayHi_Global(string name)
        {
            return _currentLimitingService.SayHi_Global(name);
        }
        [HttpGet]
        public string SayHi_Module(string name)
        {
            return _currentLimitingService.SayHi_Module(name);
        }
        [HttpGet]
        public string SayHi_TokenBucket_Action(string name)
        {
            return _currentLimitingService.SayHi_TokenBucket_Action(name);
        }
    }
}
