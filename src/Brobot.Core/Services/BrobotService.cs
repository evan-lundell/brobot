using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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

        public async Task<IEnumerable<Job>> GetJobs()
        {
            try
            {
                var response = await _client.GetStringAsync("jobs");
                return JsonConvert.DeserializeObject<IEnumerable<Job>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get jobs");
                throw new BrobotServiceException("Failed to get jobs", ex);
            }
        }

        public async Task<IEnumerable<DiscordUser>> GetDiscordUsersForChannel(ulong channelId)
        {
            try
            {
                var response = await _client.GetStringAsync($"channels/{channelId}/discordusers");
                return JsonConvert.DeserializeObject<IEnumerable<DiscordUser>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get discord users for channel {channelId}");
                throw new BrobotServiceException($"Failed to get discord users for channel {channelId}", ex);
            }
        }

        public async Task<IEnumerable<Reminder>> GetReminders(ulong? channelId = null, DateTime? reminderDateUtc = null)
        {
            try
            {
                var queryString = string.Empty;
                List<string> queryParams = new List<string>();

                if (channelId.HasValue)
                {
                    queryParams.Add($"channelId={channelId.Value.ToString()}");
                }

                if (reminderDateUtc.HasValue)
                {
                    // We don't want to include seconds for the reminder check
                    queryParams.Add($"reminderDateUtc={reminderDateUtc.Value.ToString("yyyy-MM-ddTHH:mm")}");
                }

                if (queryParams.Count > 0)
                {
                    queryString = string.Concat("?", string.Join("&", queryParams));
                }

                var response = await _client.GetStringAsync($"reminders{queryString}");
                return JsonConvert.DeserializeObject<IEnumerable<Models.Reminder>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get reminders");
                throw new BrobotServiceException("Failed to get reminders");
            }
        }

        public async Task<Reminder> UpdateReminder(Reminder reminder)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(reminder), Encoding.UTF8, "application/json");
                var response = await _client.PutAsync($"reminders/{reminder.ReminderId}", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new BrobotServiceException($"Failed to update reminder {reminder.ReminderId} with a status code of {response.StatusCode}");
                }

                var updatedReminderString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Reminder>(updatedReminderString);
            }
            catch (BrobotServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update reminder {reminder.ReminderId}");
                throw new BrobotServiceException($"Failed to update reminder {reminder.ReminderId}", ex);
            }
        }

        public async Task<JobParameter> UpdateJobParameter(int jobId, int jobParameterId, JobParameter jobParameter)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(jobParameter), Encoding.UTF8, "application/json");
                var response = await _client.PutAsync($"jobs/{jobId}/parameter/{jobParameterId}", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new BrobotServiceException($"Failed to update job parameter {jobParameterId} with a status code of {response.StatusCode}");
                }

                var updatedJobParameterString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<JobParameter>(updatedJobParameterString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update job parameter {jobParameterId}");
                throw new BrobotServiceException($"Failed to update job parameter {jobParameterId}", ex);
            }
        }
    }
}
