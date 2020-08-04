using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Brobot.Api.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models = Brobot.Core.Models;
using Entities = Brobot.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Brobot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChannelsController : BrobotControllerBase
    {
        public ChannelsController(BrobotDbContext context, ILogger<ChannelsController> logger, IMapper mapper) 
            : base(context, logger, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Channel>>> GetChannels()
        {
            try
            {
                var channels = await Context.Channels.AsNoTracking().ToListAsync();
                return Ok(Mapper.Map<IEnumerable<Entities.Channel>, IEnumerable<Models.Channel>>(channels));
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to get channels.", ex);
            }
        }

        [HttpGet("{channelId}/discordusers")]
        public async Task<ActionResult<IEnumerable<Models.DiscordUser>>> GetChannelDiscordUsers(ulong channelId)
        {
            try
            {
                var channel = await Context.Channels
                    .AsNoTracking()
                    .Include(c => c.DiscordUserChannels)
                    .ThenInclude(duc => duc.DiscordUser)
                    .SingleOrDefaultAsync(c => c.ChannelId == channelId);
                
                if (channel == null)
                {
                    return NotFound();
                }

                var discordUsers = Mapper.Map<IEnumerable<Entities.DiscordUser>, IEnumerable<Models.DiscordUser>>(channel.DiscordUserChannels.Select(duc => duc.DiscordUser));
                return Ok(discordUsers);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Failed to get discord users for channel {channelId}", ex);
            }
        }
    }
}
