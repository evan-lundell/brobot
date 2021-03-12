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

        [HttpGet("{channelId}/hotopscoreboards")]
        public async Task<ActionResult<IEnumerable<Models.HotOpScoreboard>>> GetHotOpScoreboard(ulong channelId)
        {
            try
            {
                var utcNow = DateTime.UtcNow;

                var channel = await Context.Channels
                    .AsNoTracking()
                    .Include(c => c.DiscordUserChannels)
                    .ThenInclude(duc => duc.DiscordUser)
                    .SingleOrDefaultAsync(c => c.ChannelId == channelId);

                var activeHotOps = await Context.HotOps
                    .AsNoTracking()
                    .Include(ho => ho.Owner)
                    .ThenInclude(du => du.DiscordUserChannels)
                    .AsSplitQuery()
                    .Include(ho => ho.Sessions)
                    .ThenInclude(hos => hos.DiscordUser)
                    .ThenInclude(du => du.DiscordUserChannels)
                    .Where(ho => ho.StartDateTimeUtc <= utcNow && ho.EndDateTimeUtc > utcNow)
                    .ToListAsync();

                var scoreboards = new List<Models.HotOpScoreboard>();
                foreach (var hotOp in activeHotOps)
                {
                    if (!hotOp.Owner.DiscordUserChannels.Any(duc => duc.ChannelId == channelId))
                    {
                        continue;
                    }

                    var scores = new List<Models.HotOpScore>();
                    var minuteMultiplier = 10;
                    foreach (var discordUserChannel in channel.DiscordUserChannels.Where(duc => duc.DiscordUserId != hotOp.OwnerId))
                    {
                        var score = new Models.HotOpScore
                        {
                            DiscordUserId = discordUserChannel.DiscordUserId,
                            DiscordUserName = discordUserChannel.DiscordUser.Username,
                            Score = 0
                        };

                        foreach (var userSession in hotOp.Sessions.Where(s => s.DiscordUserId == discordUserChannel.DiscordUserId))
                        {
                            if (userSession.EndDateTimeUtc.HasValue)
                            {
                                score.Score += (int)(Math.Round((userSession.EndDateTimeUtc.Value - userSession.StartDateTimeUtc).TotalMinutes, 0) * minuteMultiplier);
                            }
                            else
                            {
                                score.Score += (int)(Math.Round((utcNow - userSession.StartDateTimeUtc).TotalMinutes, 0) * minuteMultiplier);
                            }
                        }

                        scores.Add(score);
                    }

                    var scoreboard = new Models.HotOpScoreboard
                    {
                        HotOpOwnerId = hotOp.OwnerId,
                        HotOpOwnerName = hotOp.Owner.Username,
                        Scores = scores
                    };
                    scoreboards.Add(scoreboard);
                }

                return Ok(scoreboards);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Failed to get scoreboards for channel {channelId}", ex);
            }
        }
    }
}
