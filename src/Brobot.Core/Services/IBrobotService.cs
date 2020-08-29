using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Brobot.Core.Models;

namespace Brobot.Core.Services
{
    public interface IBrobotService
    {
        /// <summary>
        /// Gets a list of all the servers
        /// </summary>
        /// <returns>The list of servers</returns>
        Task<IEnumerable<Server>> GetServers();

        /// <summary>
        /// Syncs the given servers
        /// </summary>
        /// <param name="servers">The servers to sync</param>
        /// <returns>A value indicating whether or not the sync was successful</returns>
        Task<bool> SyncServers(IEnumerable<Server> servers);

        /// <summary>
        /// Gets a list of all event responses
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<EventResponse>> GetEventResponses();

        /// <summary>
        /// Gets a list of all channels
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Channel>> GetChannels();

        /// <summary>
        /// Creates a new reminder
        /// </summary>
        /// <param name="reminder">The reminder to be saved</param>
        /// <returns></returns>
        Task<Reminder> PostReminder(Reminder reminder);

        /// <summary>
        /// Gets the configured jobs
        /// </summary>
        /// <returns>The list of jobs</returns>
        Task<IEnumerable<Job>> GetJobs();

        /// <summary>
        /// Gets a list of Discord users for a given channel
        /// </summary>
        /// <param name="channelId">The channel id</param>
        /// <returns>The discord users for the given channel</returns>
        Task<IEnumerable<DiscordUser>> GetDiscordUsersForChannel(ulong channelId);

        /// <summary>
        /// Get remidners
        /// </summary>
        /// <param name="channelId">Optional channel id parameter</param>
        /// <param name="reminderDateUtc">Optional reminder date parameter</param>
        /// <returns>The reminders</returns>
        Task<IEnumerable<Reminder>> GetReminders(ulong? channelId = null, DateTime? reminderDateUtc = null);

        /// <summary>
        /// Updates a reminder
        /// </summary>
        /// <param name="reminder">The reminder to be updated</param>
        /// <returns>The updated reminder</returns>
        Task<Reminder> UpdateReminder(Reminder reminder);

        /// <summary>
        /// Updates a job parameter
        /// </summary>
        /// <param name="jobId">The job id</param>
        /// <param name="jobParameterId">The parameter id</param>
        /// <param name="jobParameter">The parameter that is being updated</param>
        /// <returns>The updated parameter</returns>
        Task<JobParameter> UpdateJobParameter(int jobId, int jobParameterId, JobParameter jobParameter);
    }
}
