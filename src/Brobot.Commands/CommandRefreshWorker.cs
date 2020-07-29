using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Brobot.Commands.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Brobot.Commands
{
    public class CommandRefreshWorker : BackgroundService
    {
        private readonly ILogger<CommandRefreshWorker> _logger;
        private readonly DiscordSocketClient _client;
        private readonly CommandHandlingService _commandHandlingService;
        private readonly CommandsSettings _commandsSettings;

        public CommandRefreshWorker(ILogger<CommandRefreshWorker> logger, 
            DiscordSocketClient client,
            CommandHandlingService commandHandlingService,
            IOptions<CommandsSettings> commandsSettings)
        {
            _logger = logger;
            _client = client;
            _commandHandlingService = commandHandlingService;
            _commandsSettings = commandsSettings.Value;
        }

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _client.Log += LogDiscordMessage;
                await _client.LoginAsync(Discord.TokenType.Bot, _commandsSettings.BrobotToken);
                await _client.StartAsync();
                await _commandHandlingService.InitializeAsync();

                await base.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start ResponsesService.");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
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
