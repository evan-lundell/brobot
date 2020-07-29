using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Brobot.Core.Exceptions;
using Brobot.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Brobot.Core.Services
{
    public class BrobotService : IBrobotService
    {
        private readonly HttpClient _client;
        private readonly ILogger<BrobotService> _logger;

        public BrobotService(HttpClient client, ILogger<BrobotService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<IEnumerable<Server>> GetServers()
        {
            try
            {
                var response = await _client.GetStringAsync("servers");
                var servers = JsonConvert.DeserializeObject<IEnumerable<Server>>(response);
                return servers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get servers");
                throw new BrobotServiceException("Failed to get servers", ex);
            }
        }

        public async Task<bool> SyncServers(IEnumerable<Server> servers)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(servers), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("servers/sync", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to sync servers. Status code: {response.StatusCode}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync servers.");
                return false;
            }
        }

        public async Task<IEnumerable<EventResponse>> GetEventResponses()
        {
            try
            {
                var response = await _client.GetStringAsync("responses");
                var responses = JsonConvert.DeserializeObject<IEnumerable<EventResponse>>(response);
                return responses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get event responses");
                throw new BrobotServiceException("Failed to get event responses", ex);
            }
        }

        public async Task<IEnumerable<Channel>> GetChannels()
        {
            try
            {
                var response = await _client.GetStringAsync("channels");
                var channels = JsonConvert.DeserializeObject<IEnumerable<Channel>>(response);
                return channels;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get channels.");
                throw new BrobotServiceException("Failed to get channels", ex);
            }
        }

        public async Task<Reminder> PostReminder(Reminder reminder)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(reminder), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("reminders", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new BrobotServiceException($"Failed to create reminder with a status code of {response.StatusCode}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Reminder>(responseContent);
            }
            catch (BrobotServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create reminder");
                throw new BrobotServiceException("Failed to create reminder", ex);
            }
        }
    }
}
