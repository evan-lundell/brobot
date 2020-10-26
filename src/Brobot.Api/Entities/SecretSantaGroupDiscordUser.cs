using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class SecretSantaGroupDiscordUser
    {
        public int SecretSantaGroupId { get; set; }
        public SecretSantaGroup SecretSantaGroup { get; set; }
        
        public ulong DiscordUserId { get; set; }
        public DiscordUser DiscordUser { get; set; }

    }
}
