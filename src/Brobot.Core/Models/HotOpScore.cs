using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class HotOpScore
    {
        public ulong DiscordUserId { get; set; }
        public string DiscordUserName { get; set; }
        public int Score { get; set; }
    }
}
