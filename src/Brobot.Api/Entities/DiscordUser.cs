using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class DiscordUser
    {
        public ulong DiscordUserId { get; set; }
        public string Username { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Timezone { get; set; }

        public ICollection<DiscordUserChannel> DiscordUserChannels { get; set; }

        public DiscordUser()
        {
            DiscordUserChannels = new HashSet<DiscordUserChannel>();
        }
    }
}
