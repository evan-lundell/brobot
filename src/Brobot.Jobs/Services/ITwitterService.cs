using Brobot.Jobs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Brobot.Jobs.Services
{
    public interface ITwitterService
    {
        Task<IEnumerable<Tweet>> GetTweetsAsync(string from, string contains = null, string sinceId = null, DateTime? startTime = null);
    }
}
