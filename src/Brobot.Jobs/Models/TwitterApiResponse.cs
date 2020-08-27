using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Jobs.Models
{
    public class TwitterApiResponse
    {
        [JsonProperty(PropertyName = "data")]
        public IEnumerable<Tweet> Data { get; set; }

        [JsonProperty(PropertyName = "meta")]
        public TwitterApiMeta Meta { get; set; }
    }
}
