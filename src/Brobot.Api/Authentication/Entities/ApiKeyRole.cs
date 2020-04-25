using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Authentication.Entities
{
    public class ApiKeyRole
    {
        public ApiKey ApiKey { get; set; }
        public int ApiKeyId { get; set; }

        public ApiRole ApiRole { get; set; }
        public int ApiRoleId { get; set; }
    }
}
