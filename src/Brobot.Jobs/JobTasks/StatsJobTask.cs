using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Brobot.Core.Models;
using Brobot.Core.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Sdcb.WordClouds;

namespace Brobot.Jobs.JobTasks
{
    public class StatsJobTask : JobTaskBase
    {
        private const string _wordcloudPath = "wordcloud.png";

        public StatsJobTask(ILogger<StatsJobTask> logger, IBrobotService brobotService, DiscordSocketClient discordClient, Job job) 
            : base(logger, brobotService, discordClient, job)
        {
        }

        internal override async Task ExecuteJobAsync(CancellationToken stoppingToken)
        {
            try
            {
                var periodParameter = Job.JobParameters.FirstOrDefault(jp => jp.Name.Equals("period", StringComparison.OrdinalIgnoreCase));
                var wordCloudParameter = Job.JobParameters.FirstOrDefault(jp => jp.Name.Equals("GenerateWordCloud", StringComparison.OrdinalIgnoreCase));
                bool.TryParse(wordCloudParameter?.Value ?? "false", out bool generateWordCloud);
                HashSet<string> stopWords = null;

                if (generateWordCloud)
                {
                    stopWords = new HashSet<string>((await BrobotService.GetStopWords()).Select(sw => sw.Word));
                }

                foreach (var channel in Job.Channels)
                {
                    var discordChannel = DiscordClient.GetChannel(channel.ChannelId);
                    if (!(discordChannel is SocketTextChannel socketTextChannel))
                    {
                        continue;
                    }

                    string[] separatingStrings = { " ", "\t", "\n", "\r\n", ",", ":", ".", "!" };

                    var messageCount = new Dictionary<ulong, (string UserName, int MessageCount)>();
                    var words = new Dictionary<string, int>();
                    foreach (var user in discordChannel.Users)
                    {
                        messageCount.Add(user.Id, (UserName: user.Username, MessageCount: 0));
                    }

                    var timePeriod = GetPeriod(periodParameter.Value);
                    var messages = await socketTextChannel.GetMessagesAsync(limit: 10000).FlattenAsync();
                    foreach (var message in messages.Where(m => m.CreatedAt.UtcDateTime >= timePeriod.PeriodStartInclusive && m.CreatedAt.UtcDateTime < timePeriod.PeriodEndExclusive))
                    {
                        if (!messageCount.TryGetValue(message.Author.Id, out (string UserName, int MessageCount) count))
                        {
                            continue;
                        }
                        count.MessageCount++;
                        messageCount[message.Author.Id] = count;
                        if (generateWordCloud)
                        {
                            foreach (var word in message.Content.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries).Where(w => !stopWords.Contains(w, StringComparer.OrdinalIgnoreCase)))
                            {
                                if (!words.ContainsKey(word))
                                {
                                    words.Add(word, 0);
                                }
                                words[word]++;
                            }
                        }
                    }

                    var builder = new StringBuilder();
                    foreach (var count in messageCount.Values.OrderByDescending(c => c.MessageCount))
                    {
                        builder.AppendLine($"{count.UserName}: {count.MessageCount}");
                    }

                    await socketTextChannel.SendMessageAsync(builder.ToString());

                    if (generateWordCloud)
                    {
                        try
                        {
                            var wc = new WordCloud(1280, 720);
                            var frequencies = words
                                .OrderByDescending(w => w.Value)
                                .Take(100)
                                .Select(w => new { Frequency = w.Value, Word = w.Key });
                            wc.Draw(frequencies.Select(f => f.Word).ToList(), frequencies.Select(f => f.Frequency).ToList()).Save(_wordcloudPath);
                            await socketTextChannel.SendFileAsync(_wordcloudPath, string.Empty);
                            File.Delete(_wordcloudPath);
                        }
                        catch (Exception ex) // if there was an error with the word cloud, don't kill the entire process
                        {
                            Logger.LogWarning(ex, "Failed to generate word cloud");
                        }
                    }
                }
            }
            catch (ArgumentException argEx)
            {
                Logger.LogError(argEx, "Invalid argument, stopping job");
                await StopAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to execute stats job.");
                NumberOfFailures++;
            }
        }

        private (DateTime PeriodStartInclusive, DateTime PeriodEndExclusive) GetPeriod(string periodType)
        {
            var utcNow = DateTime.UtcNow;
            switch (periodType.ToLower())
            {
                case "month":
                    var lastMonth = utcNow.AddMonths(-1);
                    var lastMonthStart = new DateTime(lastMonth.Year, lastMonth.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    var thisMonthStart = new DateTime(utcNow.Year, utcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    return (lastMonthStart, thisMonthStart);
                case "day":
                    var yesterday = utcNow.AddDays(-1);
                    var yesterdayStart = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 0, 0, 0, DateTimeKind.Utc);
                    var todayStart = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);
                    return (yesterdayStart, todayStart);
                case "year":
                    var lastYear = utcNow.AddYears(-1);
                    var lastYearStart = new DateTime(lastYear.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var thisYearStart = new DateTime(utcNow.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    return (lastYearStart, thisYearStart);
                default:
                    throw new ArgumentException($"{periodType} is not a supported value", "Period");
            }
            
        }
    }
}
