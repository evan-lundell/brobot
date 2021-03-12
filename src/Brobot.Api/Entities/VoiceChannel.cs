using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class VoiceChannel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public Server Server { get; set; }
        public ulong ServerId { get; set; }

        public ICollection<HotOpSession> HotOpSessions { get; set; }

        public VoiceChannel()
        {
            HotOpSessions = new HashSet<HotOpSession>();
        }
    }
}
