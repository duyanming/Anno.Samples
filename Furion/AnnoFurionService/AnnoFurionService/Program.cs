using System;
using System.Linq;
using Anno.Loader;
using Anno.Log;

//using Microsoft.Extensions.DependencyInjection;
using Autofac;

namespace AnnoFurionService
{
    using Anno.EngineData;
    using Anno.Rpc.Server;
    using SqlSugar;

    class Program
    {
        static void Main(string[] args)
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

                var services = IocLoader.GetAutoFacContainerBuilder();
                services.RegisterType(typeof(RpcConnectorImpl)).As(typeof(IRpcConnector)).SingleInstance();
                services.RegisterType(typeof(SqlSugarRepository)).As(typeof(ISqlSugarRepository)).SingleInstance();
                //var services = IocLoader.GetServiceDescriptors();
                //services.AddSingleton(typeof(IRpcConnector), typeof(RpcConnectorImpl));
            }
            , () =>
            {
                Bootstrap.ApiDoc();
            });
        }
    }
}
