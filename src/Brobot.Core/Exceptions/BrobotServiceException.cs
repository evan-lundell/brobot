using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Exceptions
{
    public class BrobotServiceException : Exception
    {
        public int? HttpErrorCode { get; set; }
        public string MessageFromServer { get; set; }

        public BrobotServiceException(int httpErrorCode, string messageFromServer, string message)
            : base(message)
        {
            HttpErrorCode = httpErrorCode;
            MessageFromServer = messageFromServer;
        }

        public BrobotServiceException(int httpErrorCode, string messageFromServer, string message, Exception innerException)
            : base(message, innerException)
        {
            HttpErrorCode = httpErrorCode;
            MessageFromServer = messageFromServer;
        }

        public BrobotServiceException(string message)
            : base(message)
        {
        }

        public BrobotServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
