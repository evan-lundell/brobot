using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class JobDefinition
    {
        public int JobDefinitionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<JobParameterDefinition> JobParameterDefinitions { get; set; }
        public ICollection<Job> Jobs { get; set; }

        public JobDefinition()
        {
            JobParameterDefinitions = new HashSet<JobParameterDefinition>();
            Jobs = new HashSet<Job>();
        }
    }
}
