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
        public bool? BrobotAdmin { get; set; }

        public ICollection<DiscordUserChannel> DiscordUserChannels { get; set; }
        public ICollection<Reminder> Reminders { get; set; }
        public ICollection<SecretSantaGroupDiscordUser> SecretSantaGroupDiscordUsers { get; set; }
        public ICollection<SecretSantaEvent> SecretSantaEvents { get; set; }
        public ICollection<SecretSantaPairing> GiverPairings { get; set; }
        public ICollection<SecretSantaPairing> RecipientPairings { get; set; }

        public DiscordUser()
        {
            DiscordUserChannels = new HashSet<DiscordUserChannel>();
            Reminders = new HashSet<Reminder>();
            SecretSantaGroupDiscordUsers = new HashSet<SecretSantaGroupDiscordUser>();
            SecretSantaEvents = new HashSet<SecretSantaEvent>();
            GiverPairings = new HashSet<SecretSantaPairing>();
            RecipientPairings = new HashSet<SecretSantaPairing>();
        }
    }
}
