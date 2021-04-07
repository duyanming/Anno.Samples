using System;
using Anno.Rpc.Client;
using Anno.Rpc.Client.DynamicProxy;
using ConsoleClient.Contract;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "模拟控制台客户端";
            Init();
            var helloAnnoService = AnnoProxyBuilder.GetService<IHelloAnnoService>();
            for (int i = 0; i < 10; i++)
            {
                var rlt = helloAnnoService.SayHi($"jack ma--[{i}]");
                Console.WriteLine(rlt);
            }
            Console.ReadLine();
        }
        /// <summary>
        /// 指定注册中心
        /// 连接池配置
        /// </summary>
        static void Init()
        {
            DefaultConfigManager.SetDefaultConnectionPool(100, Environment.ProcessorCount * 2, 20);
            DefaultConfigManager.SetDefaultConfiguration("ConsoleClient", "127.0.0.1", 7010, false);
        }
    }
}
