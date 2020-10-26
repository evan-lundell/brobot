using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class SecretSantaGroup
    {
        public int SecretSantaGroupId { get; set; }
        public string Name { get; set; }
        public bool? CheckPastYearPairings { get; set; }

        public ICollection<SecretSantaGroupDiscordUser> SecretSantaGroupDiscordUsers { get; set; }
        public ICollection<SecretSantaEvent> SecretSantaEvents { get; set; }

        public SecretSantaGroup()
        {
            SecretSantaGroupDiscordUsers = new HashSet<SecretSantaGroupDiscordUser>();
            SecretSantaEvents = new HashSet<SecretSantaEvent>();
        }
    }
}
