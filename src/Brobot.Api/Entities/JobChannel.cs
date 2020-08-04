using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class JobChannel
    {
        public Job Job { get; set; }
        public int JobId { get; set; }

        public Channel Channel { get; set; }
        public ulong ChannelId { get; set; }
    }
}
