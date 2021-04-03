using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class DailyMessageCount
    {
        public DiscordUser DiscordUser { get; set; }
        public Channel Channel { get; set; }
        public DateTime Day { get; set; }
        public int MessageCount { get; set; }
    }
}
