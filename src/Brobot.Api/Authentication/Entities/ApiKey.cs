using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Authentication.Entities
{
    public class ApiKey
    {
        public int ApiKeyId { get; set; }
        public string Owner { get; set; }
        public string Key { get; set; }
        public DateTime CreatedDateUtc { get; set; }

        public ICollection<ApiKeyRole> ApiKeyRoles { get; set; }

        public ApiKey()
        {
            ApiKeyRoles = new HashSet<ApiKeyRole>();
        }
    }
}
