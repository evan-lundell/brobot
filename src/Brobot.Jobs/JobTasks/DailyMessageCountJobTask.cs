using Brobot.Core.Models;
using Brobot.Core.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace Brobot.Jobs.JobTasks
{
    public class DailyMessageCountJobTask : JobTaskBase
    {
        public DailyMessageCountJobTask(ILogger<DailyMessageCountJobTask> logger, IBrobotService brobotService, DiscordSocketClient discordClient, Job job) 
            : base(logger, brobotService, discordClient, job)
        {
        }

        internal async override Task ExecuteJobAsync(CancellationToken stoppingToken)
        {
            try
            {
                foreach (var channel in Job.Channels)
                {
                    var discordChannel = DiscordClient.GetChannel(channel.ChannelId);
                    if (!(discordChannel is SocketTextChannel socketTextChannel))
                    {
                        continue;
                    }

                    TimeSpan yesterdayStartOffset = TimeSpan.Zero, todayStartOffset = TimeSpan.Zero;
                    var now = DateTime.UtcNow;
                    if (!string.IsNullOrWhiteSpace(channel.PrimaryTimezone))
                    {
                        var tzInfo = TZConvert.GetTimeZoneInfo(channel.PrimaryTimezone);
                        todayStartOffset = tzInfo.GetUtcOffset(now);
                        now = now + todayStartOffset;
                        yesterdayStartOffset = tzInfo.GetUtcOffset(now.AddDays(-1));
                    }

                    var yesterday = now.AddDays(-1);
                    var startDateTime = new DateTimeOffset(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0, yesterdayStartOffset);
                    var endDateTime = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, todayStartOffset);

                    var counts = new Dictionary<ulong, DailyMessageCount>();
                    foreach (var user in channel.DiscordUsers)
                    {
                        counts.Add(user.DiscordUserId, new DailyMessageCount
                        {
                            DiscordUser = user,
                            Channel = channel,
                            Day = startDateTime.DateTime,
                            MessageCount = 0
                        });
                    }

                    var messages = await socketTextChannel.GetMessagesAsync(startDateTime, endDateTime);

                    foreach (var message in messages)
                    {
                        if (message.Author.IsBot || !counts.ContainsKey(message.Author.Id))
                        {
                            continue;
                        }
                        counts[message.Author.Id].MessageCount++;
                    }

                    await BrobotService.CreateDailyMessageCounts(counts.Values);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to execute daily message count job.");
                NumberOfFailures++;
            }
        }
    }
}
