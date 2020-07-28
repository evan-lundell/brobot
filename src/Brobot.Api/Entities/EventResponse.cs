using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class EventResponse
    {
        public int EventResponseId { get; set; }
        
        public int DiscordEventId { get; set; }
        public DiscordEvent DiscordEvent { get; set; }

        public ulong? ChannelId { get; set; }
        public Channel Channel { get; set; }

        public string MessageText { get; set; }

        public string ResponseText { get; set; }
    }
}
