using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class HotOp
    {
        public int Id { get; set; }
        public DiscordUser Owner { get; set; }
        public DateTime StartDateTimeUtc { get; set; }
        public DateTime EndDateTimeUtc { get; set; }
        public ulong? PrimaryChannelId { get; set; }

        public IEnumerable<HotOpSession> Sessions { get; set; }

        public HotOp()
        {
            Sessions = new List<HotOpSession>();
        }
    }
}
