using Brobot.Commands.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Brobot.Commands.Services
{
    public class RandomFactService : IRandomFactService
    {
        private readonly ILogger<RandomFactService> _logger;
        private readonly HttpClient _client;

        public RandomFactService(ILogger<RandomFactService> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<RandomFact> GetRandomFactAsync()
        {
            try
            {
                var response = await _client.GetStringAsync("random.json?language=en");
                var fact = JsonConvert.DeserializeObject<RandomFact>(response);
                return fact;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get random fact");
                return null;
            }
        }
    }
}
