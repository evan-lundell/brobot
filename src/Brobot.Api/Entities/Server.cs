﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class Server
    {
        public ulong ServerId { get; set; }
        public string Name { get; set; }

        public ICollection<Channel> Channels { get; set; }
        public ICollection<VoiceChannel> VoiceChannels { get; set; }

        public Server()
        {
            Channels = new HashSet<Channel>();
            VoiceChannels = new HashSet<VoiceChannel>();
        }
    }
}
