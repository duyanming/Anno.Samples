using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anno.EngineData;
using Anno.Loader;
using Anno.Log;
using Anno.Rpc.Server;
using Autofac;
using Topshelf;

namespace SamplesWinService
{
    class AnnoServiceControl : ServiceControl
    {
        public bool Start(HostControl hostControl)
        {
            AnnoServiceStart(Program.Args);
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            if (Server.State)
            {
                Server.Stop();
            }
            return true;
        }

        private Task AnnoServiceStart(string[] args)
        {
            return Task.Factory.StartNew(() =>
             {
                 if (args.Contains("-help"))
                 {
                     Log.WriteLineNoDate(@"
启动参数：
                -p     6659                    设置启动端口
                -xt    200                     设置服务最大线程数
                -t     20000                   设置超时时间（单位毫秒）
                -w     1                       设置权重
                -h     192.168.0.2             设置服务在注册中心的地址
                -tr    false                   设置调用链追踪是否启用
");
                     return;
                 }
                 /**
                  * 启动默认DI库为 Autofac 可以切换为微软自带的DI库 DependencyInjection
                  */
                 Bootstrap.StartUp(args, () =>//服务配置文件读取完成后回调(服务未启动)
                 {

                     var autofac = IocLoader.GetAutoFacContainerBuilder();
                     autofac.RegisterType(typeof(RpcConnectorImpl)).As(typeof(IRpcConnector)).SingleInstance();
                 }
                 , Bootstrap.ApiDoc);
             }, TaskCreationOptions.LongRunning);
        }
    }
}
