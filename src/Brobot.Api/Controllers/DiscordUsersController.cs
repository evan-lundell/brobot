using AutoMapper;
using Brobot.Api.Contexts;
using Entities = Brobot.Api.Entities;
using Models = Brobot.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Brobot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DiscordUsersController : BrobotControllerBase
    {
        public DiscordUsersController(BrobotDbContext context, ILogger<DiscordUsersController> logger, IMapper mapper) 
            : base(context, logger, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.DiscordUser>>> GetDiscordUsers()
        {
            try
            {
                var discordUsersQuery = Context
                    .DiscordUsers
                    .AsNoTracking()
                    .AsQueryable();

                var discordUsers = await discordUsersQuery.ToListAsync();
                return Ok(Mapper.Map<IEnumerable<Entities.DiscordUser>, IEnumerable<Models.DiscordUser>>(discordUsers));
            }
            catch (Exception ex)
            {
                return InternalServerError("Unable to get Discord Users.", ex);
            }
        }

        [HttpGet("{discordUserId}")]
        public async Task<ActionResult<Models.DiscordUser>> GetDiscordUser(ulong discordUserId)
        {
            try
            {
                var discordUser = await Context.DiscordUsers.FindAsync(discordUserId);
                return Ok(Mapper.Map<Entities.DiscordUser, Models.DiscordUser>(discordUser));
            }
            catch (Exception ex)
            {
                return InternalServerError($"Unable to get Discord user {discordUserId}", ex);
            }
        }
    }
}
