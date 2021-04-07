using System;
using Anno.Rpc.Center;

namespace SamplesCenter
{
    class Program
    {

        /// <summary>
        /// 注册中心只用增加一个 Anno.config配置文件，然后直接 Bootstrap.StartUp(args);启动即可
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Title = "SamplesCenter";
            Bootstrap.StartUp(args
                , (service, noticeType) =>//上线下线
                {

                }, (newService, oldService) =>//服务配置更改
                {

                });
        }
    }
}
