using Anno.EventBus;
using SamplesEventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplesEventBus.EventHandlers
{
   public class MailSend : IEventHandler<Notice>
    {
        public void Handler(Notice entity)
        {
            Console.WriteLine($"Mail:你好{entity.Name},{entity.Msg}");
        }
    }
}
