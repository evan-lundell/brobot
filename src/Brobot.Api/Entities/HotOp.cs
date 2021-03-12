using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class HotOp
    {
        public int Id { get; set; }
        
        public ulong OwnerId { get; set; }
        public DiscordUser Owner { get; set; }
        
        public DateTime StartDateTimeUtc { get; set; }
        public DateTime EndDateTimeUtc { get; set; }

        public Channel PrimaryChannel { get; set; }
        public ulong? PrimaryChannelId { get; set; }

        public ICollection<HotOpSession> Sessions { get; set; }

        public HotOp()
        {
            Sessions = new HashSet<HotOpSession>();
        }
    }
}
