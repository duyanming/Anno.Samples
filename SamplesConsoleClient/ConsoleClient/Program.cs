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
            var sayHi = helloAnnoService.SayHi($"jack ma--[SayHi]");
            Console.WriteLine(sayHi);

            var goodBye = helloAnnoService.GoodBye($"jack ma--[GoodBye]");
            Console.WriteLine(goodBye);

            var goodByeAlias = helloAnnoService.GoodByeAlias($"jack ma--[GoodByeAlias]");
            Console.WriteLine(goodByeAlias);
            Console.WriteLine("调用完成");
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
