﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Jobs
{
    public class JobsSettings
    {
        public string BrobotToken { get; set; }
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string TwitterConsumerKey { get; set; }
        public string TwitterConsumerSecret { get; set; }
        public string TwitterAccessTokenKey { get; set; }
        public string TwitterAccessTokenSecret { get; set; }
    }
}
