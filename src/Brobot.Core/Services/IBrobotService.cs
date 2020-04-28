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
        Task<IEnumerable<Server>> GetServers();
        Task SyncServers(IEnumerable<Server> servers);
    }
}
