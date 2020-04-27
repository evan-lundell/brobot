using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class Server
    {
        public ulong ServerId { get; set; }
        public string Name { get; set; }
        public IEnumerable<Channel> Channels { get; set; }

        public Server()
        {
            Channels = new List<Channel>();
        }
    }
}
