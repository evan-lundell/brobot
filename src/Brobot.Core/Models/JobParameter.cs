using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class JobParameter
    {
        public int JobParameterId { get; set; }
        public int JobParameterDefinitionId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
