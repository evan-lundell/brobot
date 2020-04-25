using Brobot.Api.Authentication.Entities;
using Brobot.Api.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Authentication
{
    public class DbGetApiKeyQuery : IGetApiKeyQuery
    {
        private readonly AuthenticationDbContext _context;

        public DbGetApiKeyQuery(AuthenticationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiKey> Execute(string providedApiKey)
            => await _context.ApiKeys
                .Include(ak => ak.ApiKeyRoles)
                .ThenInclude(akr => akr.ApiRole)
                .SingleOrDefaultAsync(ak => ak.Key == providedApiKey);
    }
}
