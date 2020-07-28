using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class EventResponse
    {
        public int EventResponseId { get; set; }
        public string EventName { get; set; }
        public string MessageText { get; set; }
        public string ResponseText { get; set; }
        public ulong? ChannelId { get; set; }
    }
}
