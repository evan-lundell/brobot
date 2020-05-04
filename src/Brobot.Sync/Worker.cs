using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Brobot.Core.Models;
using Brobot.Core.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Brobot.Sync
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IBrobotService _brobotService;
        private readonly DiscordSocketClient _discordClient;
        private readonly SyncSettings _syncSettings;

        public Worker(
            ILogger<Worker> logger, 
            IBrobotService brobotService, 
            DiscordSocketClient discordClient,
            IOptions<SyncSettings> syncSettings)
        {
            _logger = logger;
            _brobotService = brobotService;
            _discordClient = discordClient;
            _syncSettings = syncSettings.Value;
        }

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _discordClient.LoginAsync(Discord.TokenType.Bot, _syncSettings.BrobotToken);
                await _discordClient.StartAsync();

                _discordClient.Log += LogDiscordMessage;

                await base.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start SyncService.");
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

        public async override Task StopAsync(CancellationToken cancellationToken)
        {
            _discordClient.Log -= LogDiscordMessage;

            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Give the application a couple seconds to finish the discord connect
            // TODO: Clean this up
            await Task.Delay(5000, stoppingToken);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                var servers = GetServers();
                var success = await _brobotService.SyncServers(servers);

                if (success)
                {
                    _logger.LogInformation("Successfully synced servers");
                }
                else
                {
                    _logger.LogWarning("Failed to sync servers");
                }

                await Task.Delay(_syncSettings.SyncIntervalInMinutes * 60000, stoppingToken);
            }
        }

        private IEnumerable<Server> GetServers()
        {
            var servers = new List<Server>();
            var users = new Dictionary<ulong, DiscordUser>();

            foreach (var guild in _discordClient.Guilds)
            {
                var channels = new List<Channel>();
                foreach (var channel in guild.Channels)
                {
                    if (!(channel is SocketTextChannel textChannel))
                    {
                        continue;
                    }

                    var discordUsers = new List<DiscordUser>();
                    foreach (var user in channel.Users)
                    {
                        if (user.IsBot)
                        {
                            continue;
                        }

                        if (!users.ContainsKey(user.Id))
                        {
                            users.Add(user.Id, new DiscordUser
                            {
                                DiscordUserId = user.Id,
                                Username = user.Username
                            });
                        }
                        discordUsers.Add(users[user.Id]);
                    }

                    channels.Add(new Channel
                    {
                        ChannelId = channel.Id,
                        Name = channel.Name,
                        DiscordUsers = discordUsers
                    });
                }

                servers.Add(new Server
                {
                    ServerId = guild.Id,
                    Name = guild.Name,
                    Channels = channels
                });
            }

            return servers;
        }
    }
}
