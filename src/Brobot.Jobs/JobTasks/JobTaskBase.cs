using Brobot.Core.Models;
using Brobot.Core.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brobot.Jobs.JobTasks
{
    public abstract class JobTaskBase
    {
        protected Task _executingTask;
        protected CancellationTokenSource _tokenSource;

        public Job Job { get; }

        protected ILogger Logger { get; }
        protected IBrobotService BrobotService { get; }
        protected DiscordSocketClient DiscordClient { get; }

        protected int NumberOfFailures { get; set; }

        private bool _executing;

        public JobTaskBase(ILogger logger, IBrobotService brobotService, DiscordSocketClient discordClient, Job job)
        {
            Logger = logger;
            BrobotService = brobotService;
            Job = job;
            DiscordClient = discordClient;
            NumberOfFailures = 0;
            _tokenSource = new CancellationTokenSource();
            _executing = false;
        }

        public Task StartAsync()
        {
            _executingTask = ExecuteAsync(_tokenSource.Token);

            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            return Task.CompletedTask;
        }

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var utcNow = DateTime.UtcNow;
                var schedule = CrontabSchedule.Parse(Job.CronTrigger, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
                var next = schedule.GetNextOccurrence(utcNow);
                await ExpandedDelay((next - utcNow).TotalMilliseconds, stoppingToken);
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                if (_executing)
                {
                    Logger.LogWarning($"Job {Job.Name} is still executing, skipping.");
                    continue;
                }

                _executing = true;
                await ExecuteJobAsync(stoppingToken);
                if (NumberOfFailures > 3)
                {
                    await StopAsync();
                }
                _executing = false;
            }
        }

        public async Task StopAsync()
        {
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _tokenSource.Cancel();
            }
            finally
            {
                await _executingTask;
            }
        }

        protected abstract Task ExecuteJobAsync(CancellationToken stoppingToken);

        private async Task ExpandedDelay(double milliseconds, CancellationToken stoppingToken)
        {
            // Round to the nearest millisecond
            var roundedDelay = Math.Round(milliseconds, 0);

            while (roundedDelay > 0)
            {
                var currentDelay = roundedDelay > int.MaxValue ? int.MaxValue : (int)roundedDelay;
                await Task.Delay(currentDelay, stoppingToken).ContinueWith(t => { });
                roundedDelay -= currentDelay;
            }
        }
    }
}
