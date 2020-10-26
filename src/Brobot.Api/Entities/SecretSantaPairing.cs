using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class SecretSantaPairing
    {
        public int SecretSantaPairingId { get; set; }

        public int SecretSantaEventId { get; set; }
        public SecretSantaEvent SecretSantaEvent { get; set; }

        public DiscordUser Giver { get; set; }
        public ulong GiverId { get; set; }

        public DiscordUser Recipient { get; set; }
        public ulong RecipientId { get; set; }

    }
}
