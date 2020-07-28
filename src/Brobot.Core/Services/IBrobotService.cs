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
    }
}
