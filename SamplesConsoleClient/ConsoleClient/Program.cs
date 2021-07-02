using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Anno.Rpc.Client;
using Anno.Rpc.Client.DynamicProxy;
using ConsoleClient.Contract;

namespace ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "模拟控制台客户端";
            Init();

            //await ProxyTest();
            FromBodyTest();
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

        static async Task ProxyTest()
        {
            var helloAnnoService = AnnoProxyBuilder.GetService<IHelloAnnoService>();
            var sayHi = helloAnnoService.SayHi($"jack ma--[SayHi]");
            Console.WriteLine(sayHi);


            var taskSayHi = await helloAnnoService.TaskSayHi($"jack ma--[TaskSayHi]");
            Console.WriteLine(taskSayHi);

            var goodBye = helloAnnoService.GoodBye($"jack ma--[GoodBye]");
            Console.WriteLine(goodBye);

            var goodByeAlias = helloAnnoService.GoodByeAlias($"jack ma--[GoodByeAlias]");
            Console.WriteLine(goodByeAlias);
        }

        static void FromBodyTest()
        {
            List<string> list = new List<string>();
            list.Add("test");
            list.Add("test2");
            Dictionary<string, string> input = new Dictionary<string, string>();
            input["name"] = "test";
            input["list"] = Newtonsoft.Json.JsonConvert.SerializeObject(list);

            input[Anno.Const.Enum.Eng.NAMESPACE] = "Anno.Plugs.HelloWorld";
            input[Anno.Const.Enum.Eng.CLASS] = "HelloAnno";
            input[Anno.Const.Enum.Eng.METHOD] = "Validate";
            var rlt = Connector.BrokerDns(input);
            Console.WriteLine(rlt);
        }
    }
}
