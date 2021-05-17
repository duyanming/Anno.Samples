using Anno.EventBus;
using SamplesEventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplesEventBus.EventHandlers
{
    class SMS : IEventHandler<Notice>
    {
        public void Handler(Notice entity)
        {
            Console.WriteLine($"SMS:你好{entity.Name},{entity.Msg}");
        }
    }
}
