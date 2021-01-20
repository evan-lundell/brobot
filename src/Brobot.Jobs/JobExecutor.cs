using Brobot.Core.Services;
using Brobot.Jobs.JobTasks;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Brobot.Jobs
{
    public class JobExecutor
    {
        private readonly ILogger _logger;
        private readonly DiscordSocketClient _client;
        private readonly IBrobotService _brobotService;
        private readonly IServiceProvider _serviceProvider;
        private readonly JobsSettings _jobsSettings;

        public JobExecutor(ILogger<JobExecutor> logger, 
            DiscordSocketClient client, 
            IBrobotService brobotService, 
            IServiceProvider serviceProvider,
            IOptions<JobsSettings> options)
        {
            _logger = logger;
            _client = client;
            _brobotService = brobotService;
            _serviceProvider = serviceProvider;
            _jobsSettings = options.Value;
        }

        public async Task Execute(int jobId)
        {
            try
            {
                await _client.LoginAsync(Discord.TokenType.Bot, _jobsSettings.BrobotToken);
                await _client.StartAsync();

                while (_client.LoginState != Discord.LoginState.LoggedIn)
                {
                    await Task.Delay(1000);
                }

                var job = await _brobotService.GetJob(jobId);
                if (job == null)
                {
                    _logger.LogWarning($"Job {jobId} not retrieved from brobot service");
                    return;
                }

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

                var cancellationTokenSource = new CancellationTokenSource();
                await newJobTask.ExecuteJobAsync(cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to execute job {jobId}");
            }
        }
    }
}
