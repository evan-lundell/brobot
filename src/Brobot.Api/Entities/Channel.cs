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

        public Channel()
        {
            DiscordUserChannels = new HashSet<DiscordUserChannel>();
        }
    }
}
