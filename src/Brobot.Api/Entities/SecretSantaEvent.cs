using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class SecretSantaEvent
    {
        public int SecretSantaEventId { get; set; }
        public int Year { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public DiscordUser CreatedBy { get; set; }
        public ulong? CreatedById { get; set; }
        public SecretSantaGroup SecretSantaGroup { get; set; }
        public int SecretSantaGroupId { get; set; }

        public ICollection<SecretSantaPairing> SecretSantaPairings { get; set; }

        public SecretSantaEvent()
        {
            SecretSantaPairings = new HashSet<SecretSantaPairing>();
        }
    }
}
