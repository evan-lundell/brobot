using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Brobot.Jobs.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Brobot.Jobs.Services
{
    public class TwitterService : ITwitterService
    {
        private readonly HttpClient _client;
        private readonly ILogger<TwitterService> _logger;

        public TwitterService(HttpClient client, ILogger<TwitterService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<IEnumerable<Tweet>> GetTweetsAsync(string from, string contains = null, string sinceId = null, DateTime? startTime = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(from))
                {
                    throw new ArgumentNullException(nameof(from));
                }

                var queryStringBuilder = HttpUtility.ParseQueryString(string.Empty);
                queryStringBuilder.Add("tweet.fields", "created_at");
                  
                var filterText = $"from:{from}";
                if (!string.IsNullOrWhiteSpace(contains))
                {
                    filterText += $" {contains}";
                }
                queryStringBuilder.Add("query", filterText);

                if (!string.IsNullOrWhiteSpace(sinceId))
                {
                    queryStringBuilder.Add("since_id", sinceId);
                }

                if (startTime.HasValue)
                {
                    queryStringBuilder.Add("start_time", startTime.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                }

                var tweets = new List<Tweet>();
                bool moreResults = true;
                var nextToken = string.Empty;
                while (moreResults)
                {
                    var response = await _client.GetStringAsync($"tweets/search/recent?{queryStringBuilder.ToString()}");
                    var twitterApiResponse = JsonConvert.DeserializeObject<TwitterApiResponse>(response, new JsonSerializerSettings
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat
                    });

                    if (twitterApiResponse.Data != null)
                    {
                        tweets.AddRange(twitterApiResponse.Data);
                    }

                    if (!string.IsNullOrWhiteSpace(twitterApiResponse.Meta.NextToken))
                    {
                        queryStringBuilder.Set("next_token", twitterApiResponse.Meta.NextToken);
                    }
                    else
                    {
                        moreResults = false;
                    }
                }

                return tweets;
            }
            catch (ArgumentNullException argEx)
            {
                _logger.LogError(argEx, $"{argEx.ParamName} cannot be null");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get tweets for {from}");
                throw;
            }
        }
    }
}
