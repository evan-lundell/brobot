using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brobot.Jobs
{
    public static class Extensions
    {
        public async static Task<IEnumerable<IMessage>> GetMessagesAsync(this SocketTextChannel socketTextChannel, DateTimeOffset startDateTime, DateTimeOffset endDateTime)
        {
            IMessage previous = null;
            var messagesInDateRange = new List<IMessage>();
            while (previous == null || previous.Timestamp > startDateTime)
            {
                List<IMessage> messages;
                if (previous == null)
                {
                    messages = (await socketTextChannel.GetMessagesAsync().FlattenAsync()).OrderBy(m => m.Timestamp).ToList();
                }
                else
                {
                    messages = (await socketTextChannel.GetMessagesAsync(previous, Direction.Before).FlattenAsync()).OrderBy(m => m.Timestamp).ToList();
                }
                if (messages.Count == 0)
                {
                    break;
                }

                previous = messages[0];
                messagesInDateRange.AddRange(messages.Where(m => m.Timestamp >= startDateTime && m.Timestamp < endDateTime));
            }

            return messagesInDateRange;
        }
    }
}
