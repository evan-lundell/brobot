using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Brobot.Core.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Brobot.Jobs.JobTasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Brobot.Core.Models;

namespace Brobot.Jobs
{
    public class JobsWorker : BackgroundService
    {
        private readonly ILogger<JobsWorker> _logger;
        private readonly IBrobotService _brobotService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly DiscordSocketClient _discordClient;
        private readonly JobsSettings _jobsSettings;
        private List<JobTaskBase> _jobTasks;

        public JobsWorker(ILogger<JobsWorker> logger, 
            IBrobotService brobotService, 
            IServiceProvider serviceProvider,
            IHostApplicationLifetime hostApplicationLifetime,
            IOptions<JobsSettings> options,
            DiscordSocketClient discordClient)
        {
            _logger = logger;
            _brobotService = brobotService;
            _serviceProvider = serviceProvider;
            _hostApplicationLifetime = hostApplicationLifetime;
            _discordClient = discordClient;
            _jobsSettings = options.Value;
            _jobTasks = new List<JobTaskBase>();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _discordClient.LoginAsync(Discord.TokenType.Bot, _jobsSettings.BrobotToken);
                await _discordClient.StartAsync();

                _discordClient.Log += LogDiscordMessage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start JobsService.");
            }

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var retryCount = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                var wasSuccessful = await CheckJobs();
                if (wasSuccessful)
                {
                    retryCount = 0;
                }
                else
                {
                    retryCount++;
                }

                if (retryCount > 3)
                {
                    _logger.LogCritical("Retry count met. Shutting down.");
                    await StopAsync(stoppingToken);
                    _hostApplicationLifetime.StopApplication();
                    break;
                }

                await Task.Delay(60000);
            }
        }

        /// <summary>
        /// Checks the jobs and create/update/removes running jobs as necessary
        /// </summary>
        /// <returns>Indicates whether or not the check was successful</returns>
        private async Task<bool> CheckJobs()
        {
            try
            {
                var jobs = await _brobotService.GetJobs();
                foreach (var job in jobs)
                {
                    var jobTask = _jobTasks.FirstOrDefault(jt => jt.Job.JobId == job.JobId);
                    if (jobTask == null)
                    {
                        await CreateNewJobTask(job);
                    }
                    else
                    {
                        if ((jobTask.Job.ModifiedDateUtc == null && job.ModifiedDateUtc.HasValue)
                            || (jobTask.Job.ModifiedDateUtc.HasValue && job.ModifiedDateUtc.HasValue && jobTask.Job.ModifiedDateUtc.Value < job.ModifiedDateUtc.Value))
                        {
                            await jobTask.StopAsync();
                            _jobTasks.Remove(jobTask);
                            await CreateNewJobTask(job);
                        }
                    }
                }

                foreach (var jobTask in _jobTasks.Where(jt => !jobs.Any(j => j.JobId == jt.Job.JobId)))
                {
                    await jobTask.StopAsync();
                    _jobTasks.Remove(jobTask);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check jobs.");
                return false;
            }
        }

        private async Task CreateNewJobTask(Job job)
        {
            var type = Type.GetType($"Brobot.Jobs.JobTasks.{job.JobDefinition.Name}JobTask");
            if (type == null)
            {
                _logger.LogWarning($"Unabled to create instance of {job.JobDefinition.Name} job");
                return;
            }
            var newJobTask = ActivatorUtilities.CreateInstance(_serviceProvider, type, job) as JobTaskBase;
            if (newJobTask == null)
            {
                _logger.LogWarning($"Unable to activate instance of {job.JobDefinition.Name} job");
                return;
            }
            await newJobTask.StartAsync();
            _jobTasks.Add(newJobTask);
        }

        private Task LogDiscordMessage(Discord.LogMessage logMessage)
        {
            switch (logMessage.Severity)
            {
                case Discord.LogSeverity.Critical:
                    _logger.LogCritical(logMessage.Exception, logMessage.Message);
                    break;
                case Discord.LogSeverity.Debug:
                    _logger.LogDebug(logMessage.Message);
                    break;
                case Discord.LogSeverity.Error:
                    _logger.LogError(logMessage.Exception, logMessage.Message);
                    break;
                case Discord.LogSeverity.Info:
                    _logger.LogInformation(logMessage.Message);
                    break;
                case Discord.LogSeverity.Verbose:
                    _logger.LogTrace(logMessage.Message);
                    break;
                case Discord.LogSeverity.Warning:
                    _logger.LogWarning(logMessage.Exception, logMessage.Message);
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
