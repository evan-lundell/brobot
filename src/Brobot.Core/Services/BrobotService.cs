using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
                return new List<Server>();
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
                return new List<EventResponse>();
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
                return new List<Channel>();
            }
        }
    }
}
