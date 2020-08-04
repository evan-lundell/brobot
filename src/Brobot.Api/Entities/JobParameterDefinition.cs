using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class JobParameterDefinition
    {
        public int JobParameterDefinitionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        public bool UserConfigurable { get; set; }
        public string DataType { get; set; }

        public JobDefinition JobDefinition { get; set; }
        public int JobDefinitionId { get; set; }

        public ICollection<JobParameter> JobParameters { get; set; }

        public JobParameterDefinition()
        {
            JobParameters = new HashSet<JobParameter>();
        }
    }
}
