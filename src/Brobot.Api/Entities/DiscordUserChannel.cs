using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class DiscordUserChannel
    {
        public Channel Channel { get; set; }
        public ulong ChannelId { get; set; }

        public DiscordUser DiscordUser { get; set; }
        public ulong DiscordUserId { get; set; }
    }
}
