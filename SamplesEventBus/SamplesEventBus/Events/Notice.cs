using Anno.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplesEventBus.Events
{
    public class Notice : EventData
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Msg { get; set; }
    }
}
