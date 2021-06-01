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
using TimeZoneConverter;

namespace Brobot.Jobs.JobTasks
{
    public class MonthlyStatsReportJobTask : JobTaskBase
    {
        private const string _wordcloudPath = "wordcloud.png";

        public MonthlyStatsReportJobTask(ILogger<MonthlyStatsReportJobTask> logger, IBrobotService brobotService, DiscordSocketClient discordClient, Job job) 
            : base(logger, brobotService, discordClient, job)
        {
        }

        internal override async Task ExecuteJobAsync(CancellationToken stoppingToken)
        {
            try
            {
                var wordCloudParameter = Job.JobParameters.FirstOrDefault(jp => jp.Name.Equals("GenerateWordCloud", StringComparison.OrdinalIgnoreCase));
                bool.TryParse(wordCloudParameter?.Value ?? "false", out bool generateWordCloud);
                HashSet<string> stopWords = null;

                var now = DateTime.UtcNow;
                var startOfThisMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
                var startOfLastMonth = startOfThisMonth.AddMonths(-1);

                if (generateWordCloud)
                {
                    stopWords = new HashSet<string>((await BrobotService.GetStopWords()).Select(sw => sw.Word));
                }
                string[] separatingStrings = { " ", "\t", "\n", "\r\n", ",", ":", ".", "!", "/", "\\", "%", "&", "?", "(", ")", "\"", "@" };


                foreach (var channel in Job.Channels)
                {
                    TimeSpan lastMonthOffset = TimeSpan.Zero, currentOffset = TimeSpan.Zero;
                    if (!string.IsNullOrWhiteSpace(channel.PrimaryTimezone))
                    {
                        var tzInfo = TZConvert.GetTimeZoneInfo(channel.PrimaryTimezone);
                        lastMonthOffset = tzInfo.GetUtcOffset(startOfLastMonth);
                        currentOffset = tzInfo.GetUtcOffset(startOfThisMonth);
                    }
                    var startDateTime = new DateTimeOffset(startOfLastMonth, lastMonthOffset);
                    var endDateTime = new DateTimeOffset(startOfThisMonth, currentOffset);

                    var discordChannel = DiscordClient.GetChannel(channel.ChannelId);
                    if (!(discordChannel is SocketTextChannel socketTextChannel))
                    {
                        continue;
                    }

                    var messageCount = new Dictionary<ulong, (string UserName, int MessageCount)>();
                    var words = new Dictionary<string, int>();
                    foreach (var user in discordChannel.Users)
                    {
                        messageCount.Add(user.Id, (UserName: user.Username, MessageCount: 0));
                    }

                    foreach (var message in await socketTextChannel.GetMessagesAsync(startDateTime, endDateTime))
                    {
                        if (messageCount.TryGetValue(message.Author.Id, out var count))
                        {
                            count.MessageCount++;
                            messageCount[message.Author.Id] = count;
                        }

                        if (generateWordCloud)
                        {
                            foreach (var word in message.Content.Split(separatingStrings, StringSplitOptions.RemoveEmptyEntries).Where(w => !stopWords.Contains(w, StringComparer.OrdinalIgnoreCase)))
                            {
                                if (!words.ContainsKey(word.ToLower()))
                                {
                                    words.Add(word.ToLower(), 0);
                                }
                                words[word.ToLower()]++;
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
    }
}
