using Brobot.Core.Models;
using Brobot.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Brobot.Sync
{
    public class SyncService
    {
        private readonly IBrobotService _brobotService;

        public SyncService(IBrobotService brobotService)
        {
            _brobotService = brobotService;
        }

        public async Task RunAsync()
        {
            var servers = new List<Server>
            {
                new Server
                {
                    ServerId = 421404457599762433,
                    Name = "Lundells",
                    Channels = new List<Channel>
                    {
                        new Channel
                        {
                            ChannelId = 500317523346980865,
                            Name = "brobot-test",
                            DiscordUsers = new List<DiscordUser>
                            {
                                new DiscordUser
                                {
                                    DiscordUserId = 144477184017301505,
                                    Username = "Evan"
                                }
                            }
                        }
                    }
                }
            };

            await _brobotService.SyncServers(servers);
        }
    }
}
