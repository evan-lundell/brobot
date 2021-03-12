using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class HotOpSession
    {
        public int Id { get; set; }
       
        public ulong DiscordUserId { get; set; }
        public DiscordUser DiscordUser { get; set; }

        public ulong VoiceChannelId { get; set; }
        public VoiceChannel VoiceChannel { get; set; }

        public int HotOpId { get; set; }
        public HotOp HotOp { get; set; }

        public DateTime StartDateTimeUtc { get; set; }
        public DateTime? EndDateTimeUtc { get; set; }
    }
}
