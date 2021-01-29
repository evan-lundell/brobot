using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brobot.Commands.Models
{
    internal static class PollEmoji
    {
        public static Dictionary<int, string> DiscordCode = new Dictionary<int, string>
        {
            { 1, ":one:" },
            { 2, ":two:" },
            { 3, ":three:" },
            { 4, ":four:" },
            { 5, ":five:" },
            { 6, ":six:" },
            { 7, ":seven:" },
            { 8, ":eight:" },
            { 9, ":nine:" },
            { 10, ":keycap_ten:" }
        };

        public static Dictionary<int, string> Unicode = new Dictionary<int, string>
        {
            { 1, "1️⃣" },
            { 2, "2️⃣" },
            { 3, "3️⃣" },
            { 4, "4️⃣" },
            { 5, "5️⃣" },
            { 6, "6️⃣" },
            { 7, "7️⃣" },
            { 8, "8️⃣" },
            { 9, "9️⃣" },
            { 10, "🔟" }
        };
    }
}
