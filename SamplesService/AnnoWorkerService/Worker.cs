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
����������
                -p     6659                    ���������˿�
                -xt    200                     ���÷�������߳���
                -t     20000                   ���ó�ʱʱ�䣨��λ���룩
                -w     1                       ����Ȩ��
                -h     192.168.0.2             ���÷�����ע�����ĵĵ�ַ
                -tr    false                   ���õ�����׷���Ƿ�����
");
                    return;
                }
                /**
                 * ����Ĭ��DI��Ϊ Autofac �����л�Ϊ΢���Դ���DI�� DependencyInjection
                 */
                Bootstrap.StartUp(args, () =>//���������ļ���ȡ��ɺ�ص�(����δ����)
                {

                    var autofac = IocLoader.GetAutoFacContainerBuilder();
                    autofac.RegisterType(typeof(RpcConnectorImpl)).As(typeof(IRpcConnector)).SingleInstance();
                }
                , Bootstrap.ApiDoc);
            }, TaskCreationOptions.LongRunning);
        }
    }
}
