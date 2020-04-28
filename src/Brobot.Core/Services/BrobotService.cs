using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Brobot.Core.Models;
using Newtonsoft.Json;

namespace Brobot.Core.Services
{
    public class BrobotService : IBrobotService
    {
        private readonly HttpClient _client;

        public BrobotService(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<Server>> GetServers()
        {
            var response = await _client.GetStringAsync("servers");
            var servers = JsonConvert.DeserializeObject<IEnumerable<Server>>(response);
            return servers;
        }

        public async Task SyncServers(IEnumerable<Server> servers)
        {
            var content = new StringContent(JsonConvert.SerializeObject(servers), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("servers/sync", content);
        }
    }
}
