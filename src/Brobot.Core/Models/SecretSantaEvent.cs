using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class SecretSantaEvent
    {
        public int SecretSantaEventId { get; set; }
        public int Year { get; set; }
        public DateTime? CreatedDateUtc { get; set; }
        public ulong? CreatedById { get; set; }
        public int SecretSantaGroupId { get; set; }

        public ICollection<SecretSantaPairing> SecretSantaPairings { get; set; }

        public SecretSantaEvent()
        {
            SecretSantaPairings = new List<SecretSantaPairing>();
        }
    }
}
