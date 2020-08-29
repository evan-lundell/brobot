using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Jobs.Models
{
    public class TwitterApiMeta
    {
        [JsonProperty(PropertyName = "newest_id")]
        public string NewestId { get; set; }

        [JsonProperty(PropertyName = "oldest_id")]
        public string OldestId { get; set; }

        [JsonProperty(PropertyName = "result_count")]
        public int ResultCount { get; set; }

        [JsonProperty(PropertyName = "next_token")]
        public string NextToken { get; set; }
    }
}
