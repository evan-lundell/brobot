using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class Reminder
    {
        public int ReminderId { get; set; }
        public ulong OwnerId { get; set; }
        public ulong ChannelId { get; set; }
        public string Message { get; set; }
        public DateTime ReminderDateUtc { get; set; }
        public DateTime? SentDateUtc { get; set; }
    }
}
