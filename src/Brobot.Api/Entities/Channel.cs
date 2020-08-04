using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class Channel
    {
        public ulong ChannelId { get; set; }
        public string Name { get; set; }

        public Server Server { get; set; }
        public ulong ServerId { get; set; }

        public ICollection<DiscordUserChannel> DiscordUserChannels { get; set; }
        public ICollection<EventResponse> EventResponses { get; set; }
        public ICollection<Reminder> Reminders { get; set; }
        public ICollection<JobChannel> JobChannels { get; set; }

        public Channel()
        {
            DiscordUserChannels = new HashSet<DiscordUserChannel>();
            EventResponses = new HashSet<EventResponse>();
            Reminders = new HashSet<Reminder>();
            JobChannels = new HashSet<JobChannel>();
        }
    }
}
