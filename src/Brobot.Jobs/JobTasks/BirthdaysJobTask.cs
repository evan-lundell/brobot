using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Brobot.Core.Models;
using Brobot.Core.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Brobot.Jobs.JobTasks
{
    public class BirthdaysJobTask : JobTaskBase
    {
        public BirthdaysJobTask(ILogger<BirthdaysJobTask> logger, IBrobotService brobotService, DiscordSocketClient discordSocketClient, Job job) 
            : base(logger, brobotService, discordSocketClient, job)
        {
        }

        protected async override Task ExecuteJobAsync(CancellationToken stoppingToken)
        {
            try
            {
                var today = DateTime.Now;
                foreach (var channel in Job.Channels)
                {
                    var discordChannel = DiscordClient.GetChannel(channel.ChannelId);
                    if (!(discordChannel is SocketTextChannel socketTextChannel))
                    {
                        continue;
                    }
                    var channelUsers = await BrobotService.GetDiscordUsersForChannel(channel.ChannelId);
                    foreach (var user in channelUsers.Where(cu => cu.Birthdate.HasValue 
                        && cu.Birthdate.Value.Month == today.Month 
                        && cu.Birthdate.Value.Day == today.Day))
                    {
                        var discordUser = socketTextChannel.GetUser(user.DiscordUserId);
                        if (discordUser == null)
                        {
                            continue;
                        }
                        await socketTextChannel.SendMessageAsync($"Happy Birthday {discordUser.Mention}! :birthday:");
                        NumberOfFailures = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to execute birthday job.");
                NumberOfFailures++;
            }
        }
    }
}
