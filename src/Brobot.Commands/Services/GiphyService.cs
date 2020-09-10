using Brobot.Commands.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Brobot.Commands.Services
{
    public class GiphyService : IGiphyService
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private readonly CommandsSettings _commandsSettings;

        public GiphyService(HttpClient client, ILogger<GiphyService> logger, IOptions<CommandsSettings> options)
        {
            _client = client;
            _logger = logger;
            _commandsSettings = options.Value;
        }

        public async Task<GiphyResponse> GetRandomGif(string tag = null)
        {
            try
            {
                var queryStringBuilder = HttpUtility.ParseQueryString(string.Empty);
                queryStringBuilder.Add("api_key", _commandsSettings.GiphyApiKey);
                if (!string.IsNullOrEmpty(tag))
                {
                    queryStringBuilder.Add("tag", tag);
                }
                queryStringBuilder.Add("rating", "pg-13");

                var response = await _client.GetStringAsync($"random?{queryStringBuilder.ToString()}");
                var giphyResponse = JsonConvert.DeserializeObject<GiphyResponse>(response);
                return giphyResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get random gif");
                return null;
            }
        }
    }
}
