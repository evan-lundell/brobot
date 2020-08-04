using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class JobParameterDefinition
    {
        public int JobParameterDefinitionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        public bool UserConfigurable { get; set; }
        public string DataType { get; set; }
    }
}
