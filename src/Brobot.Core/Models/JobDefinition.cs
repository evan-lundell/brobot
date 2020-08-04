using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class JobDefinition
    {
        public int JobDefinitionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<JobParameterDefinition> JobParameterDefinitions { get; set; }

        public JobDefinition()
        {
            JobParameterDefinitions = new List<JobParameterDefinition>();
        }
    }
}
