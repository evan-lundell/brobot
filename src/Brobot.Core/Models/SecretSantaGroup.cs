using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Models
{
    public class SecretSantaGroup
    {
        public int SecretSantaGroupId { get; set; }
        public string Name { get; set; }
        public bool CheckPastYearPairings { get; set; }
    }
}
