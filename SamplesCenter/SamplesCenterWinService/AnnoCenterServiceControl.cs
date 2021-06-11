using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anno.Rpc.Center;
using Topshelf;

namespace SamplesCenterWinService
{
    class AnnoCenterServiceControl : ServiceControl
    {
        public bool Start(HostControl hostControl)
        {
            AnnoCenterStart(Program.Args);
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            if (Monitor.State)
            {
                Monitor.Stop();
            }
            return true;
        }

        private Task AnnoCenterStart(string[] args)
        {
            return Task.Factory.StartNew(() =>
             {
                 Bootstrap.StartUp(args
                     , (service, noticeType) =>//上线下线
                     {

                     }, (newService, oldService) =>//服务配置更改
                     {

                     });
             }, TaskCreationOptions.LongRunning);
        }
    }
}
