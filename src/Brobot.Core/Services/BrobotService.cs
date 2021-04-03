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

        public async Task<SecretSantaGroup> CreateSecretSantaGroup(SecretSantaGroup secretSantaGroup)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(secretSantaGroup), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("secretsanta/group", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessageString = await response.Content.ReadAsStringAsync();
                    var errorMessage = JsonConvert.DeserializeObject<BrobotServiceError>(errorMessageString);
                    throw new BrobotServiceException((int)response.StatusCode, errorMessage.Message, $"Failed to create secret santa group {secretSantaGroup.Name}");
                }

                var newSecretSantaGroupString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SecretSantaGroup>(newSecretSantaGroupString);
            }
            catch (BrobotServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create secret santa group {secretSantaGroup.Name}");
                throw new BrobotServiceException($"Failed to create secret santa group {secretSantaGroup.Name}", ex);
            }
        }

        public async Task<SecretSantaGroupDiscordUser> AddDiscordUserToSecretSantaGroup(int groupId, ulong discordUserId)
        {
            try
            {
                var secretSantaGroupDiscordUser = new SecretSantaGroupDiscordUser
                {
                    SecretSantaGroupId = groupId,
                    DiscordUserId = discordUserId
                };
                var content = new StringContent(JsonConvert.SerializeObject(secretSantaGroupDiscordUser), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"secretsanta/group/{groupId}/member", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessageString = await response.Content.ReadAsStringAsync();
                    var errorMessage = JsonConvert.DeserializeObject<BrobotServiceError>(errorMessageString);
                    throw new BrobotServiceException((int)response.StatusCode, errorMessage.Message, $"Failed to add user {discordUserId} to secret santa group {groupId}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SecretSantaGroupDiscordUser>(responseString);
            }
            catch (BrobotServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to add user {discordUserId} to secret santa group {groupId}");
                throw new BrobotServiceException($"Failed to add user {discordUserId} to secret santa group {groupId}", ex);
            }
        }

        public async Task<SecretSantaEvent> CreateSecretSantaEvent(int groupId, int year, ulong? createdById)
        {
            try
            {
                var ssEvent = new SecretSantaEvent
                {
                    SecretSantaGroupId = groupId,
                    Year = year,
                    CreatedById = createdById
                };
                var content = new StringContent(JsonConvert.SerializeObject(ssEvent), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"secretsanta/group/{groupId}/event", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessageString = await response.Content.ReadAsStringAsync();
                    var errorMessage = JsonConvert.DeserializeObject<BrobotServiceError>(errorMessageString);
                    throw new BrobotServiceException((int)response.StatusCode, errorMessage.Message, $"Failed to create event for group {groupId}, year {year}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SecretSantaEvent>(responseString);
            }
            catch (BrobotServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create event for group {groupId}, year {year}");
                throw new BrobotServiceException($"Failed to create event for group {groupId}, year {year}", ex);
            }
        }

        public async Task<DiscordUser> GetDiscordUser(ulong discordUserId)
        {
            try
            {
                var response = await _client.GetStringAsync($"discordusers/{discordUserId}");
                return JsonConvert.DeserializeObject<DiscordUser>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get user {discordUserId}");
                throw new BrobotServiceException($"Failed to get user {discordUserId}", ex);
            }
        }

        public async Task<Job> GetJob(int jobId)
        {
            try
            {
                var response = await _client.GetStringAsync($"jobs/{jobId}");
                return JsonConvert.DeserializeObject<Job>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get job {jobId}");
                throw new BrobotServiceException($"Failed to get user {jobId}", ex);
            }
        }

        public async Task<IEnumerable<StopWord>> GetStopWords()
        {
            try
            {
                var response = await _client.GetStringAsync("words/stopwords");
                return JsonConvert.DeserializeObject<IEnumerable<StopWord>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get stop words");
                throw new BrobotServiceException("Failed to get stop words", ex);
            }
        }

        public async Task<IEnumerable<HotOp>> GetHotOps(bool activeOnly = false, DateTime? startDateTimeUtc = null, DateTime? endDateTimeUtc = null)
        {
            try
            {
                var queryString = string.Empty;
                List<string> queryParams = new List<string>();

                if (activeOnly)
                {
                    queryParams.Add($"activeOnly=true");
                }

                if (startDateTimeUtc.HasValue)
                {
                    // We don't want to include seconds for the start date check
                    queryParams.Add($"startDateTimeUtc={startDateTimeUtc.Value.ToString("yyyy-MM-ddTHH:mm")}");
                }

                if (endDateTimeUtc.HasValue)
                {
                    // We don't want to include seconds for the end date check
                    queryParams.Add($"endDateTimeUtc={endDateTimeUtc.Value.ToString("yyyy-MM-ddTHH:mm")}");
                }

                if (queryParams.Count > 0)
                {
                    queryString = string.Concat("?", string.Join("&", queryParams));
                }

                var response = await _client.GetStringAsync($"hotops{queryString}");
                return JsonConvert.DeserializeObject<IEnumerable<HotOp>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get hot ops");
                throw new BrobotServiceException("Failed to get hot ops", ex);
            }
        }

        public async Task<HotOpSession> CreateHotOpSession(int hotOpId, HotOpSession session)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(session), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync($"hotops/{hotOpId}/sessions", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new BrobotServiceException($"Failed to create hot op session for hot op {hotOpId} with a status code of {response.StatusCode}");
                }

                var sessionResponseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<HotOpSession>(sessionResponseString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create hot op session");
                throw new BrobotServiceException("Failed to create hot op session", ex);
            }
        }

        public async Task<HotOpSession> UpdateHotOpSession(int hotOpId, HotOpSession session)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(session), Encoding.UTF8, "application/json");
                var response = await _client.PutAsync($"hotops/{hotOpId}/sessions/{session.Id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new BrobotServiceException($"Failed to update hot op session {session.Id} for hot op {hotOpId} with a status code of {response.StatusCode}");
                }

                var sessionResponseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<HotOpSession>(sessionResponseString);
            }
            catch (BrobotServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update hot op session");
                throw new BrobotServiceException("Failed to update hot op session", ex);
            }
        }

        public async Task<IEnumerable<HotOpScoreboard>> GetHotOpScoreboards(ulong channelId)
        {
            try
            {
                var response = await _client.GetStringAsync($"channels/{channelId}/hotopscoreboards");
                return JsonConvert.DeserializeObject<IEnumerable<HotOpScoreboard>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update hot op session");
                throw new BrobotServiceException("Failed to update hot op session", ex);
            }
        }

        public async Task<HotOpScoreboard> GetHotOpScoreboard(int id, ulong? channelId = null)
        {
            try
            {
                string queryString = "";
                if (channelId.HasValue)
                {
                    queryString = $"?channelId={channelId}";
                }
                var response = await _client.GetStringAsync($"hotops/{id}/scoreboard" + queryString);
                return JsonConvert.DeserializeObject<HotOpScoreboard>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update hot op session");
                throw new BrobotServiceException("Failed to update hot op session", ex);
            }
        }

        public async Task<HotOp> CreateHotOp(HotOp hotOp)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(hotOp), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("hotops", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new BrobotServiceException($"Failed to create a hot op with the status code of {response.StatusCode}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<HotOp>(responseString);
            }
            catch (BrobotServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create hot op");
                throw new BrobotServiceException("Failed to create hot op", ex);
            }
        }

        public async Task<IEnumerable<DailyMessageCount>> GetDailyMessageCounts(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var queryString = "";
                var queryStringParams = new List<string>();
                
                if (startDate.HasValue)
                {
                    queryStringParams.Add($"startDate={startDate.Value.ToString("yyyy-MM-dd")}");
                }

                if (endDate.HasValue)
                {
                    queryStringParams.Add($"endDate={endDate.Value.ToString("yyyy-MM-dd")}");
                }

                if (queryStringParams.Count > 0)
                {
                    queryString = string.Concat("?", string.Join("&", queryStringParams));
                }

                var response = await _client.GetStringAsync($"messages{queryString}");
                return JsonConvert.DeserializeObject<IEnumerable<DailyMessageCount>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get daily message counts");
                throw new BrobotServiceException("Failed to get daily message counts", ex);
            }
        }

        public async Task<IEnumerable<DailyMessageCount>> CreateDailyMessageCounts(IEnumerable<DailyMessageCount> dailyMessageCounts)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(dailyMessageCounts), Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("messages", content);
                if (!response.IsSuccessStatusCode)
                {
                    throw new BrobotServiceException($"Failed to create daily message counts with a status code of {response.StatusCode}");
                }

                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<DailyMessageCount>>(responseString);
            }
            catch (BrobotServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create daily message counts");
                throw new BrobotServiceException("Failed to create daily message counts", ex);
            }
        }
    }
}
