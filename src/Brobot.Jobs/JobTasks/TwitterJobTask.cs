using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Brobot.Core.Models;
using Brobot.Core.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Brobot.Jobs.JobTasks
{
    public class TwitterJobTask : JobTaskBase
    {
        public TwitterJobTask(ILogger<TwitterJobTask> logger, IBrobotService brobotService, DiscordSocketClient discordClient, Job job) 
            : base(logger, brobotService, discordClient, job)
        {
        }

        protected async override Task ExecuteJobAsync(CancellationToken stoppingToken)
        {
            try
            {

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
