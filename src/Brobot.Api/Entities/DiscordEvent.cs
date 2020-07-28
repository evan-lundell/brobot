using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class DiscordEvent
    {
        public int DiscordEventId { get; set; }
        public string Name { get; set; }

        public ICollection<EventResponse> EventResponses { get; set; }

        public DiscordEvent()
        {
            EventResponses = new HashSet<EventResponse>();
        }
    }
}
