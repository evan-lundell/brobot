using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class Reminder
    {
        public int ReminderId { get; set; }
        
        public ulong OwnerId { get; set; }
        public DiscordUser Owner { get; set; }

        public ulong ChannelId { get; set; }
        public Channel Channel { get; set; }

        public string Message { get; set; }
        
        public DateTime CreatedDateUtc { get; set; }
        public DateTime ReminderDateUtc { get; set; }
        public DateTime? SentDateUtc { get; set; }
    }
}
