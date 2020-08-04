using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Entities
{
    public class JobParameter
    {
        public int JobParameterId { get; set; }
        public string Value { get; set; }

        public Job Job { get; set; }
        public int JobId { get; set; }

        public JobParameterDefinition JobParameterDefinition { get; set; }
        public int JobParameterDefinitionId { get; set; }
    }
}
