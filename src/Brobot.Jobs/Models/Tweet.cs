using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Jobs.Models
{
    public class Tweet
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
