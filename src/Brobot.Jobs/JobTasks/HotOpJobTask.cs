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

namespace Brobot.Jobs.JobTasks
{
    public class HotOpJobTask : JobTaskBase
    {
        public HotOpJobTask(ILogger<HotOpJobTask> logger, IBrobotService brobotService, DiscordSocketClient discordClient, Job job) 
            : base(logger, brobotService, discordClient, job)
        {
        }

        internal override async Task ExecuteJobAsync(CancellationToken stoppingToken)
        {
            try
            {
                var startingHotOps = await BrobotService.GetHotOps(startDateTimeUtc: DateTime.UtcNow);
                foreach (var hotOp in startingHotOps.Where(ho => ho.PrimaryChannelId.HasValue && Job.Channels.Any(c => c.ChannelId == ho.PrimaryChannelId.Value)))
                {
                    var channel = DiscordClient.GetChannel(hotOp.PrimaryChannelId.Value);
                    if (!(channel is SocketTextChannel textChannel))
                    {
                        continue;
                    }

                    await textChannel.SendMessageAsync($"Operation Hot {hotOp.Owner.Username} has begun!");
                }

                var expiringHotOps = await BrobotService.GetHotOps(endDateTimeUtc: DateTime.UtcNow);

                foreach (var hotOp in expiringHotOps)
                {
                    var utcNow = DateTime.UtcNow;
                    foreach (var openSession in hotOp.Sessions.Where(s => s.EndDateTimeUtc == null))
                    {
                        openSession.EndDateTimeUtc = utcNow;
                        await BrobotService.UpdateHotOpSession(hotOp.Id, openSession);
                    }

                    if (hotOp.PrimaryChannelId == null || !Job.Channels.Any(c => c.ChannelId == hotOp.PrimaryChannelId))
                    {
                        continue;
                    }

                    var channel = DiscordClient.GetChannel(hotOp.PrimaryChannelId.Value);
                    if (!(channel is SocketTextChannel textChannel))
                    {
                        continue;
                    }

                    var scoreboard = await BrobotService.GetHotOpScoreboard(hotOp.Id);
                    var scores = new List<HotOpScore>(scoreboard.Scores);
                    foreach (var user in textChannel.Users.Where(u => !u.IsBot && u.Id != scoreboard.HotOpOwnerId && !scoreboard.Scores.Any(s => s.DiscordUserId == u.Id)))
                    {
                        scores.Add(new HotOpScore
                        {
                            DiscordUserId = user.Id,
                            DiscordUserName = user.Username,
                            Score = 0
                        });
                    }

                    var builder = new EmbedBuilder
                    {
                        Color = new Color(114, 137, 218),
                        Description = $"Operation Hot {scoreboard.HotOpOwnerName}"
                    };

                    foreach (var score in scores.OrderByDescending(s => s.Score))
                    {
                        builder.AddField(x =>
                        {
                            x.Name = score.DiscordUserName;
                            x.Value = score.Score;
                            x.IsInline = false;
                        });
                    }

                    await textChannel.SendMessageAsync($"Operation Hot Op {scoreboard.HotOpOwnerName} has finished. Final score:", false, builder.Build());
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to start hot op job");
                NumberOfFailures++;
            }
        }

    }
}
