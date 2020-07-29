using System;
using System.Collections.Generic;
using System.Text;

namespace Brobot.Core.Exceptions
{
    public class BrobotServiceException : Exception
    {
        public BrobotServiceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
