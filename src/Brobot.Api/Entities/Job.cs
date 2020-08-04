using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class Job
    {
        public int JobId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CronTrigger { get; set; }
        public DateTime CreatedDateUtc { get; set; }
        public DateTime? ModifiedDateUtc { get; set; }

        public JobDefinition JobDefinition { get; set; }
        public int JobDefinitionId { get; set; }

        public ICollection<JobChannel> JobChannels { get; set; }
        public ICollection<JobParameter> JobParameters { get; set; }

        public Job()
        {
            JobChannels = new HashSet<JobChannel>();
        }
    }
}
