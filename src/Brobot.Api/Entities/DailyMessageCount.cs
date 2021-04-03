using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class DailyMessageCount
    {
        public ulong DiscordUserId { get; set; }
        public DiscordUser DiscordUser { get; set; }

        public ulong ChannelId { get; set; }
        public Channel Channel { get; set; }

        public DateTime Day { get; set; }

        public int MessageCount { get; set; }
    }
}
