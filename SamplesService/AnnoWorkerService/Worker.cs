using Anno.EngineData;
using Anno.Loader;
using Anno.Log;
using Anno.Rpc.Server;
using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AnnoWorkerService
{
    public class Worker : BackgroundService
    {
        public Worker()
        {
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                Log.Info($"Worker running  ExecuteAsync at: {DateTime.Now}");
            });

        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            AnnoServiceStart(Program.Args).Wait();
            Log.Info($"Worker running StartAsync at: {DateTime.Now}");
            return base.StartAsync(cancellationToken);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (Server.State)
            {
                Server.Stop();
            }
            Log.Info($"Worker stopped StopAsync at: {DateTime.Now}");
            return base.StopAsync(cancellationToken);
        }
        public override void Dispose()
        {
            Log.Info($"Worker disposed at: {DateTime.Now}");
            base.Dispose();
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
