# 🎄Anno 注册中心
    一行代码的注册中心,其他服务都会把信息注册到注册中心并且从注册中心获取服务信息

![Dashboard](https://z3.ax1x.com/2021/04/01/cE4QPS.png)
![Dashboard](https://z3.ax1x.com/2021/04/01/cE58JO.png)

# 配置文件
    配置文件 Anno.config

Port:为注册中心的监听端口
TimeOut:为注册中心默认连接超时时间单位毫秒

```C#
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
```