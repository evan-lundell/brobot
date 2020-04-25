using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Authentication.Entities
{
    public class ApiRole
    {
        public int ApiRoleId { get; set; }
        public string Name { get; set; }

        public ICollection<ApiKeyRole> ApiKeyRoles { get; set; }

        public ApiRole()
        {
            ApiKeyRoles = new HashSet<ApiKeyRole>();
        }
    }
}
