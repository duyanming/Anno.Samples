using Furion.Application.AnnoContract;
using Furion.DynamicApiController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Furion.Application
{
    public class CurrentLimitingService : IDynamicApiController,ICurrentLimitingService
    {
        private readonly ICurrentLimitingService _currentLimitingService;

        public CurrentLimitingService(ICurrentLimitingService currentLimitingService)
        {
            _currentLimitingService = currentLimitingService;
        }

        public string GoodBye_Action(string name)
        {
            return _currentLimitingService.GoodBye_Action(name);
        }

        public string GoodBye_Global(string name)
        {
            return _currentLimitingService.GoodBye_Global(name);
        }

        public string GoodBye_Module(string name)
        {
            return _currentLimitingService.GoodBye_Module(name);
        }

        public string SayHi_Global(string name)
        {
            return _currentLimitingService.SayHi_Global(name);
        }

        public string SayHi_Module(string name)
        {
            return _currentLimitingService.SayHi_Module(name);
        }

        public string SayHi_TokenBucket_Action(string name)
        {
            return _currentLimitingService.SayHi_TokenBucket_Action(name);
        }
    }
}
