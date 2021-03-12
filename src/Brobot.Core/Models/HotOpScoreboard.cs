using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class HotOpScoreboard
    {
        public ulong HotOpOwnerId { get; set; }
        public string HotOpOwnerName { get; set; }
        public IEnumerable<HotOpScore> Scores { get; set; }

        public HotOpScoreboard()
        {
            Scores = new List<HotOpScore>();
        }
    }
}
