using Anno.EventBus;
using SamplesEventBus.Events;
using System;
using System.Collections.Generic;

namespace SamplesEventBus
{
    class Program
    {
        static void Main(string[] args)
        {
            IEventBus bus = EventBus.Instance;
            #region 1、通过程序集方式

            var assembles = new List<System.Reflection.Assembly>();
            assembles.Add(typeof(Program).Assembly);
            bus.SubscribeAll(assembles);//注册指定程序集下的所有Handler

            #endregion
            #region 2、手动注册方式

            //bus.Subscribe(typeof(Notice), new EventHandlers.MailSend());

            #endregion
            bus.ErrorNotice += (string exchange, string routingKey, Exception exception, string message) =>
            {
                //Handler 抛出异常的时候 会触发

            };
            Notice notice = new Notice()
            {
                Id = 1100,
                Name = "Anno",
                Msg = "后天放假，祝节假日快乐！"
            };

            bus.Publish(notice);
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
