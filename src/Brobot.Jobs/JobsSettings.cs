using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Jobs
{
    public class JobsSettings
    {
        public string BrobotToken { get; set; }
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string TwitterApiBaseUrl { get; set; }
        public string TwitterBearerToken { get; set; }
        public string AwsAccessKey { get; set; }
        public string AwsSecretKey { get; set; }
    }
}
