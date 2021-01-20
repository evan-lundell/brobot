using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Brobot.Core.Models;
using Brobot.Core.Services;
using Brobot.Jobs.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Brobot.Jobs.JobTasks
{
    public class TwitterJobTask : JobTaskBase
    {
        private readonly ITwitterService _twitterService;

        public TwitterJobTask(ILogger<TwitterJobTask> logger, IBrobotService brobotService, DiscordSocketClient discordClient, Job job, ITwitterService twitterService) 
            : base(logger, brobotService, discordClient, job)
        {
            _twitterService = twitterService;
        }

        internal async override Task ExecuteJobAsync(CancellationToken stoppingToken)
        {
            try
            {
                var containsParameter = Job.JobParameters.FirstOrDefault(p => p.Name.Equals("contains", StringComparison.OrdinalIgnoreCase));
                var handlerParameter = Job.JobParameters.FirstOrDefault(p => p.Name.Equals("twitterhandle", StringComparison.OrdinalIgnoreCase));
                var lastCheckParameter = Job.JobParameters.FirstOrDefault(p => p.Name.Equals("lastcheckdatetimeutc", StringComparison.OrdinalIgnoreCase));

                if (string.IsNullOrWhiteSpace(handlerParameter?.Value))
                {
                    Logger.LogError($"Job {Job.Name} does not contain required parameter TwitterHandler. Exiting job");
                    NumberOfFailures = MaxNumberOfFailures;
                    return;
                }

                if (string.IsNullOrWhiteSpace(lastCheckParameter?.Value))
                {
                    Logger.LogError($"Job {Job.Name} does not contain required parameter LatestTweetId. Exiting job");
                    NumberOfFailures = MaxNumberOfFailures;
                    return;
                }

                if (!DateTime.TryParse(lastCheckParameter.Value, out DateTime latestCheck))
                {
                    Logger.LogError($"Job {Job.Name}: {lastCheckParameter.Name} is not a datetime. Exiting job");
                    NumberOfFailures = MaxNumberOfFailures;
                    return;
                }


                var tweets = await _twitterService.GetTweetsAsync(from: handlerParameter?.Value, contains: containsParameter?.Value, startTime: latestCheck.ToUniversalTime());
                var newCheckTime = DateTime.UtcNow;

                foreach (var channel in Job.Channels)
                {
                    var discordChannel = DiscordClient.GetChannel(channel.ChannelId);
                    if (!(discordChannel is SocketTextChannel socketTextChannel))
                    {
                        continue;
                    }

                    foreach (var tweet in tweets.OrderBy(t => t.CreatedAt))
                    {
                        await socketTextChannel.SendMessageAsync($"https://www.twitter.com/{handlerParameter.Value}/status/{tweet.Id}");
                    }
                }

                lastCheckParameter.Value = newCheckTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                await BrobotService.UpdateJobParameter(Job.JobId, lastCheckParameter.JobParameterId, lastCheckParameter);


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
