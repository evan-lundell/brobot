using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class SecretSantaPairing
    {
        public int SecretSantaPairingId { get; set; }
        public DiscordUser Giver { get; set; }
        public DiscordUser Recipient { get; set; }
    }
}
