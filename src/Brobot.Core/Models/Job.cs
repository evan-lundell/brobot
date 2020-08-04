using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
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
        public IEnumerable<JobParameter> JobParameters { get; set; }
        public IEnumerable<Channel> Channels { get; set; }

        public Job()
        {
            JobParameters = new List<JobParameter>();
            Channels = new List<Channel>();
        }
    }
}
