using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class HotOpSession
    {
        public int Id { get; set; }
        public ulong DiscordUserId { get; set; }
        public ulong VoiceChannelId { get; set; }
        public int HotOpId { get; set; }
        public DateTime StartDateTimeUtc { get; set; }
        public DateTime? EndDateTimeUtc { get; set; }
    }
}
