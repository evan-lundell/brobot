using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Brobot.Core.Exceptions;
using Brobot.Core.Models;
using Brobot.Core.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Brobot.Monitor
{
    public class MonitorWorker : BackgroundService
    {
        private readonly ILogger<MonitorWorker> _logger;
        private readonly DiscordSocketClient _discordClient;
        private readonly IBrobotService _brobotService;
        private readonly MonitorSettings _monitorSettings;
        private readonly IHostApplicationLifetime _applicationLifetime;

        private List<Channel> _channels;
        private Dictionary<ulong, Dictionary<string, List<EventResponse>>> _eventResponses;

        private const int _maxRetries = 3;

        public MonitorWorker(ILogger<MonitorWorker> logger, 
            DiscordSocketClient discordClient, 
            IBrobotService brobotService,
            IOptions<MonitorSettings> monitorSettings,
            IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            _discordClient = discordClient;
            _brobotService = brobotService;
            _monitorSettings = monitorSettings.Value;
            _applicationLifetime = applicationLifetime;
            _eventResponses = new Dictionary<ulong, Dictionary<string, List<EventResponse>>>();
            _channels = new List<Channel>();
        }

        public async override Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _discordClient.LoginAsync(Discord.TokenType.Bot, _monitorSettings.BrobotToken);
                await _discordClient.StartAsync();

                _discordClient.Log += LogDiscordMessage;
                _discordClient.MessageReceived += MessageReceived;

                _discordClient.ChannelUpdated += (oldChannel, newChannel) =>
                {
                    Task.Run(async () => await ChannelUpdated(oldChannel, newChannel));
                    return Task.CompletedTask;
                };

                _discordClient.MessageDeleted += (cachedMessage, channel) =>
                {
                    Task.Run(async () => await MessageDeleted(cachedMessage, channel));
                    return Task.CompletedTask;
                };

                _discordClient.UserVoiceStateUpdated += UserVoiceStateUpdated;
                await base.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start ResponsesService.");
            }
        }

        private Task UserVoiceStateUpdated(SocketUser user, SocketVoiceState oldVoiceState, SocketVoiceState newVoiceState)
        {
            Task.Run(async () => await CheckHotOps(user, oldVoiceState, newVoiceState));
            return Task.CompletedTask;
        }

        public async override Task StopAsync(CancellationToken cancellationToken)
        {
            _discordClient.Log -= LogDiscordMessage;
            await base.StopAsync(cancellationToken);
        }
        private async Task MessageDeleted(Cacheable<IMessage, ulong> cachedMessage, ISocketMessageChannel channel)
        {
            try
            {
                if (!cachedMessage.HasValue)
                {
                    return;
                }

                foreach (var response in _eventResponses[channel.Id]["MessageDeleted"])
                {
                    await channel.SendMessageAsync(response.ResponseText.Replace("{authorName}", cachedMessage.Value.Author.Username));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to respond to message deleted event.", ex);
            }
        }

        private async Task ChannelUpdated(SocketChannel oldChannel, SocketChannel newChannel)
        {
            if (!(oldChannel is SocketTextChannel oldTextChannel) || !(newChannel is SocketTextChannel newTextChannel))
            {
                return;
            }

            try
            {
                foreach (var response in _eventResponses[newTextChannel.Id]["ChannelUpdated"])
                {
                    await newTextChannel.SendMessageAsync(response.ResponseText
                        .Replace("{oldChannelName}", oldTextChannel.Name)
                        .Replace("{newChannelName}", newTextChannel.Name));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to respond to channel update event.", ex);
            }
        }

        private Task MessageReceived(SocketMessage socketMessage)
        {
            Task.Run(async () => await CheckResponse(socketMessage));
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int retryCount = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _channels.Clear();
                    _channels.AddRange(await _brobotService.GetChannels());
                    var responses = await _brobotService.GetEventResponses();
                    SetEventResponses(responses);

                    retryCount = 0;
                    await Task.Delay(60000, stoppingToken);
                }
                catch (BrobotServiceException)
                {
                    if (retryCount > _maxRetries)
                    {
                        _logger.LogCritical("Brobot service max retry count met, shutting down.");
                        _applicationLifetime.StopApplication();
                        await StopAsync(stoppingToken);
                        break;
                    }
                    else
                    {
                        retryCount++;
                        _logger.LogWarning($"Brobot service failed. Trying again in {retryCount * 10} seconds.");
                        await Task.Delay(10000 * retryCount, stoppingToken);
                    }
                }
            }
        }

        private void SetEventResponses(IEnumerable<EventResponse> eventResponses)
        {
            _eventResponses.Clear();
            foreach (var channel in _channels)
            {
                _eventResponses.Add(channel.ChannelId, new Dictionary<string, List<EventResponse>>());
                _eventResponses[channel.ChannelId].Add("MessageReceived", new List<EventResponse>());
                _eventResponses[channel.ChannelId].Add("ChannelUpdated", new List<EventResponse>());
                _eventResponses[channel.ChannelId].Add("MessageDeleted", new List<EventResponse>());
            }

            foreach (var eventResponse in eventResponses)
            {
                if (!eventResponse.ChannelId.HasValue)
                {
                    foreach (var channelId in _eventResponses.Keys)
                    {
                        _eventResponses[channelId][eventResponse.EventName].Add(eventResponse);
                    }
                }
                else
                {
                    _eventResponses[eventResponse.ChannelId.Value][eventResponse.EventName].Add(eventResponse);
                }
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

        private async Task CheckHotOps(SocketUser user, SocketVoiceState oldVoiceState, SocketVoiceState newVoiceState)
        {
            try
            {
                if (user.IsBot)
                {
                    return;
                }

                var hotOps = await _brobotService.GetHotOps(true);
                foreach (var hotOp in hotOps)
                {
                    if (user.Id == hotOp.Owner.DiscordUserId)
                    {
                        if (oldVoiceState.VoiceChannel == null || (newVoiceState.VoiceChannel != null && oldVoiceState.VoiceChannel?.Id != newVoiceState.VoiceChannel?.Id))
                        {
                            foreach (var connectedUser in newVoiceState.VoiceChannel.Users.Where(u => u.VoiceChannel.Id == newVoiceState.VoiceChannel.Id))
                            {
                                if (connectedUser.Id == user.Id || connectedUser.IsBot)
                                {
                                    continue;
                                }

                                var hotOpSession = new HotOpSession
                                {
                                    HotOpId = hotOp.Id,
                                    DiscordUserId = connectedUser.Id,
                                    StartDateTimeUtc = DateTime.UtcNow,
                                    EndDateTimeUtc = null,
                                    VoiceChannelId = newVoiceState.VoiceChannel.Id
                                };

                                await _brobotService.CreateHotOpSession(hotOp.Id, hotOpSession);
                            }
                        }

                        if (newVoiceState.VoiceChannel == null || (oldVoiceState.VoiceChannel != null && oldVoiceState.VoiceChannel?.Id != newVoiceState.VoiceChannel?.Id))
                        {
                            foreach (var connectedUser in oldVoiceState.VoiceChannel.Users.Where(u => u.VoiceChannel.Id == oldVoiceState.VoiceChannel.Id))
                            {
                                if (connectedUser.IsBot)
                                {
                                    continue;
                                }

                                var hotOpSession = hotOp.Sessions.FirstOrDefault(hos => hos.DiscordUserId == connectedUser.Id
                                    && hos.VoiceChannelId == oldVoiceState.VoiceChannel.Id
                                    && hos.EndDateTimeUtc == null);
                                if (hotOpSession == null)
                                {
                                    _logger.LogWarning($"User {connectedUser.Id} does not have a current session for hot op {hotOp.Id}");
                                    continue;
                                }

                                hotOpSession.EndDateTimeUtc = DateTime.UtcNow;
                                await _brobotService.UpdateHotOpSession(hotOp.Id, hotOpSession);
                            }
                        }
                    }
                    else
                    {
                        if (oldVoiceState.VoiceChannel == null || (newVoiceState.VoiceChannel != null && oldVoiceState.VoiceChannel?.Id != newVoiceState.VoiceChannel?.Id))
                        {
                            if (!newVoiceState.VoiceChannel.Users.Any(u => u.Id == hotOp.Owner.DiscordUserId && u.VoiceChannel.Id == newVoiceState.VoiceChannel.Id))
                            {
                                continue;
                            }

                            var hotOpSession = new HotOpSession
                            {
                                HotOpId = hotOp.Id,
                                StartDateTimeUtc = DateTime.UtcNow,
                                DiscordUserId = user.Id,
                                VoiceChannelId = newVoiceState.VoiceChannel.Id
                            };
                            await _brobotService.CreateHotOpSession(hotOp.Id, hotOpSession);
                        }

                        if (newVoiceState.VoiceChannel == null || (oldVoiceState.VoiceChannel != null && oldVoiceState.VoiceChannel?.Id != newVoiceState.VoiceChannel?.Id))
                        {
                            if (!oldVoiceState.VoiceChannel.Users.Any(u => u.Id == hotOp.Owner.DiscordUserId && u.VoiceChannel.Id == oldVoiceState.VoiceChannel.Id))
                            {
                                continue;
                            }

                            var hotOpSession = hotOp.Sessions.FirstOrDefault(hos => hos.DiscordUserId == user.Id
                                    && hos.VoiceChannelId == oldVoiceState.VoiceChannel.Id
                                    && hos.EndDateTimeUtc == null);
                            if (hotOpSession == null)
                            {
                                _logger.LogWarning($"User {user.Id} does not have a current session for hot op {hotOp.Id}");
                                continue;
                            }

                            hotOpSession.EndDateTimeUtc = DateTime.UtcNow;
                            await _brobotService.UpdateHotOpSession(hotOp.Id, hotOpSession);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to log hot op information for user {user.Id}");
            }
        }

        private async Task CheckResponse(SocketMessage socketMessage)
        {
            try
            {
                if (socketMessage.Author.IsBot)
                {
                    return;
                }

                var responses = _eventResponses[socketMessage.Channel.Id]["MessageReceived"];
                foreach (var response in responses)
                {
                    if (string.IsNullOrWhiteSpace(response.MessageText)
                        || response.MessageText.Equals(socketMessage.Content, StringComparison.OrdinalIgnoreCase))
                    {
                        await socketMessage.Channel.SendMessageAsync(response.ResponseText);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to send message received response.", ex);
            }
        }
    }
}
