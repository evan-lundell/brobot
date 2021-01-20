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
    public class RemindersJobTask : JobTaskBase
    {
        public RemindersJobTask(ILogger<RemindersJobTask> logger, IBrobotService brobotService, DiscordSocketClient discordClient, Job job) : base(logger, brobotService, discordClient, job)
        {
        }

        internal override async Task ExecuteJobAsync(CancellationToken stoppingToken)
        {
            try
            {
                var activeReminders = await BrobotService.GetReminders(reminderDateUtc: DateTime.UtcNow);
                foreach (var channel in Job.Channels)
                {
                    var discordChannel = DiscordClient.GetChannel(channel.ChannelId);
                    if (!(discordChannel is SocketTextChannel socketTextChannel))
                    {
                        continue;
                    }

                    foreach (var reminder in activeReminders.Where(r => r.ChannelId == channel.ChannelId && r.SentDateUtc == null))
                    {
                        await socketTextChannel.SendMessageAsync($"@everyone {reminder.Message}");
                        reminder.SentDateUtc = DateTime.UtcNow;
                        await BrobotService.UpdateReminder(reminder);
                    }
                }
                NumberOfFailures = 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to start reminders job");
                NumberOfFailures++;
            }
        }
    }
}
