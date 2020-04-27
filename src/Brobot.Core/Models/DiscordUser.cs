using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class DiscordUser
    {
        public ulong DiscordUserId { get; set; }
        public string Username { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Timezone { get; set; }
    }
}
