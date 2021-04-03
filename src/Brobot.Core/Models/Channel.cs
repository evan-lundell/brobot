using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class Channel
    {
        public ulong ChannelId { get; set; }
        public string Name { get; set; }
        public string PrimaryTimezone { get; set; }

        public IEnumerable<DiscordUser> DiscordUsers { get; set; }

        public Channel()
        {
            DiscordUsers = new List<DiscordUser>();
        }
    }
}
