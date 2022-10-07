using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Brobot.Commands.Models;
using Newtonsoft.Json;

namespace Brobot.Commands.Services
{
    public class DefinitionService : IDefinitionService
    {
        private readonly ILogger<DefinitionService> _logger;
        private readonly HttpClient _client;

        public DefinitionService(ILogger<DefinitionService> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        public async Task<string> GetDefinitions(string word)
        {
            try
            {
                var response = await _client.GetAsync($"entries/en/{word}");
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return "That's not a word dummy";
                }

                if (!response.IsSuccessStatusCode)
                {
                    return $"Failed to find word {word}";
                }

                var responseString = await response.Content.ReadAsStringAsync();
                var dictionaryResults = JsonConvert.DeserializeObject<DictionaryResponse[]>(responseString);
                if (dictionaryResults.Length == 0)
                {
                    return $"Failed to find word {word}";
                }

                var dictionaryResult = dictionaryResults[0];
                var definitionString = new StringBuilder();
                var count = 1;
                foreach (var meaning in dictionaryResult.Meanings)
                {
                    definitionString.AppendLine($"{count}: ({meaning.PartOfSpeech}) {meaning.Definitions[0].Definition}");
                    count++;
                }
                return definitionString.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get definition");
                return $"Unable to get definition for {word}";
            }
        }
  }
}
