using AutoMapper;
using Brobot.Api.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models = Brobot.Core.Models;
using Entities = Brobot.Api.Entities;
using TimeZoneConverter;

namespace Brobot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HotOpsController : BrobotControllerBase
    {
        public HotOpsController(BrobotDbContext context, ILogger<HotOpsController> logger, IMapper mapper)
            : base(context, logger, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.HotOp>>> GetHotOps([FromQuery]bool activeOnly, 
            [FromQuery]DateTime? startDateTimeUtc = null,
            [FromQuery]DateTime? endDateTimeUtc = null)
        {
            try
            {
                IQueryable<Entities.HotOp> hotOps = Context.HotOps
                    .AsNoTracking()
                    .Include(ho => ho.Sessions)
                    .ThenInclude(hos => hos.DiscordUser)
                    .Include(ho => ho.Owner);
                if (activeOnly)
                {
                    var utcNow = DateTime.UtcNow;
                    hotOps = hotOps.Where(ho => ho.StartDateTimeUtc <= utcNow && ho.EndDateTimeUtc > utcNow);
                }

                if (startDateTimeUtc.HasValue)
                {
                    hotOps = hotOps.Where(ho => ho.StartDateTimeUtc >= startDateTimeUtc.Value && ho.StartDateTimeUtc < startDateTimeUtc.Value.AddMinutes(1));
                }

                if (endDateTimeUtc.HasValue)
                {
                    hotOps = hotOps.Where(ho => ho.EndDateTimeUtc >= endDateTimeUtc.Value && ho.EndDateTimeUtc < endDateTimeUtc.Value.AddMinutes(1));
                }

                var hotOpsEntities = await hotOps.ToListAsync();
                return Ok(Mapper.Map<IEnumerable<Entities.HotOp>, IEnumerable<Models.HotOp>>(hotOpsEntities));
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to get hot ops", ex);
            }
        }

        [HttpGet("{id}/scoreboard")]
        public async Task<ActionResult<Models.HotOpScoreboard>> GetScoreboard(int id, [FromQuery]ulong? channelId = null)
        {
            try
            {
                var hotOp = await Context.HotOps
                    .AsNoTracking()
                    .Include(ho => ho.Owner)
                    .Include(ho => ho.Sessions)
                    .ThenInclude(hos => hos.DiscordUser)
                    .ThenInclude(du => du.DiscordUserChannels)
                    .SingleOrDefaultAsync(ho => ho.Id == id);

                if (hotOp == null)
                {
                    return NotFound($"Hot op {id} not found");
                }

                var scores = new Dictionary<ulong, Models.HotOpScore>();
                var minuteMultiplier = 10;
                var utcNow = DateTime.UtcNow;
                foreach (var session in hotOp.Sessions)
                {
                    if (channelId.HasValue && !session.DiscordUser.DiscordUserChannels.Any(duc => duc.ChannelId == channelId.Value))
                    {
                        continue;
                    }

                    if (!scores.ContainsKey(session.DiscordUserId))
                    {
                        scores.Add(session.DiscordUserId, new Models.HotOpScore
                        {
                            DiscordUserId = session.DiscordUserId,
                            DiscordUserName = session.DiscordUser.Username,
                            Score = 0
                        });
                    }

                    if (session.EndDateTimeUtc.HasValue)
                    {
                        scores[session.DiscordUserId].Score += (int)(Math.Round((session.EndDateTimeUtc.Value - session.StartDateTimeUtc).TotalMinutes, 0) * minuteMultiplier);
                    }
                    else
                    {
                        scores[session.DiscordUserId].Score += (int)(Math.Round((utcNow - session.StartDateTimeUtc).TotalMinutes, 0) * minuteMultiplier);
                    }
                }

                var scoreboard = new Models.HotOpScoreboard
                {
                    HotOpOwnerId = hotOp.OwnerId,
                    HotOpOwnerName = hotOp.Owner.Username,
                    Scores = scores.Values
                };

                return Ok(scoreboard);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Failed to get hot op {id}", ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Models.HotOp>> CreateHotOp([FromBody]Models.HotOp hotOpModel)
        {
            try
            {
                if (hotOpModel.EndDateTimeUtc <= hotOpModel.StartDateTimeUtc)
                {
                    return BadRequest(new { message = "End Date must be after start date" });
                }

                if (hotOpModel.EndDateTimeUtc <= DateTime.UtcNow)
                {
                    return BadRequest(new { message = "Invalid dates" });
                }

                if (await Context.HotOps.AnyAsync(ho => ho.OwnerId == hotOpModel.Owner.DiscordUserId
                    && ((hotOpModel.StartDateTimeUtc >= ho.StartDateTimeUtc && hotOpModel.StartDateTimeUtc < ho.EndDateTimeUtc)
                    || (hotOpModel.EndDateTimeUtc >= ho.StartDateTimeUtc && hotOpModel.EndDateTimeUtc < ho.EndDateTimeUtc))))
                {
                    return BadRequest(new { message = $"Overlapping hot ops for user {hotOpModel.Owner} " });
                }

                var discordUser = await Context.DiscordUsers.FindAsync(hotOpModel.Owner.DiscordUserId);
                if (discordUser == null)
                {
                    return BadRequest(new { message = $"Discord user {hotOpModel.Owner.DiscordUserId } does not exist" });
                }

                if (hotOpModel.PrimaryChannelId.HasValue && !await Context.Channels.AnyAsync(c => c.ChannelId == hotOpModel.PrimaryChannelId))
                {
                    return BadRequest(new { message = $"Channel {hotOpModel.PrimaryChannelId} does not exist" });
                }

                var startDateTime = hotOpModel.StartDateTimeUtc;
                var endDateTime = hotOpModel.EndDateTimeUtc;
                if (!string.IsNullOrEmpty(discordUser.Timezone))
                {
                    var timeZoneInfo = TZConvert.GetTimeZoneInfo(discordUser.Timezone);
                    startDateTime = startDateTime - timeZoneInfo.GetUtcOffset(DateTime.Now);
                    endDateTime = endDateTime - timeZoneInfo.GetUtcOffset(DateTime.Now);
                }

                var hotOpEntity = new Entities.HotOp
                {
                    OwnerId = hotOpModel.Owner.DiscordUserId,
                    StartDateTimeUtc = startDateTime,
                    EndDateTimeUtc = endDateTime,
                    PrimaryChannelId = hotOpModel.PrimaryChannelId
                };

                await Context.HotOps.AddAsync(hotOpEntity);
                await Context.SaveChangesAsync();

                hotOpModel.Id = hotOpEntity.Id;
                hotOpModel.Owner = Mapper.Map<Entities.DiscordUser, Models.DiscordUser>(discordUser);
                return Ok(hotOpModel);
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to create hot op", ex);
            }
        }
        
        [HttpPost("{hotOpId}/sessions")]
        public async Task<ActionResult<Models.HotOpSession>> CreateHotOpSession(int hotOpId, [FromBody]Models.HotOpSession hotOpSessionModel)
        {
            try
            {
                var hotOp = await Context.HotOps
                    .Include(ho => ho.Sessions)
                    .SingleOrDefaultAsync(ho => ho.Id == hotOpId);

                if (hotOp == null)
                {
                    return NotFound(new { message = $"Hot op {hotOpId} not found" });
                }
                var hotOpSessionEntity = Mapper.Map<Models.HotOpSession, Entities.HotOpSession>(hotOpSessionModel);
                hotOp.Sessions.Add(hotOpSessionEntity);
                await Context.SaveChangesAsync();
                hotOpSessionModel.Id = hotOpSessionEntity.Id;
                return Ok(hotOpSessionModel);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Failed to create session for hot op {hotOpId}", ex);
            }
        }

        [HttpPut("{hotOpId}/sessions/{hotOpSessionId}")]
        public async Task<ActionResult<Models.HotOpSession>> UpdateHotOpSession(int hotOpId, int hotOpSessionId, [FromBody]Models.HotOpSession hotOpSessionModel)
        {
            try
            {
                var hotOpEntity = await Context.HotOps
                    .Include(ho => ho.Sessions)
                    .SingleOrDefaultAsync(ho => ho.Id == hotOpId);

                if (hotOpEntity == null)
                {
                    return NotFound(new { message = $"Hot op {hotOpId} not found" });
                }

                var hotOpSessionEntity = hotOpEntity.Sessions.FirstOrDefault(hos => hos.Id == hotOpSessionId);
                if (hotOpSessionEntity == null)
                {
                    return NotFound(new { message = $"Hot op session {hotOpSessionId} not found" });
                }

                hotOpSessionEntity.StartDateTimeUtc = hotOpSessionModel.StartDateTimeUtc;
                hotOpSessionEntity.EndDateTimeUtc = hotOpSessionModel.EndDateTimeUtc;
                await Context.SaveChangesAsync();
                return Ok(hotOpSessionModel);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Failed to update session {hotOpSessionId}", ex);
            }
        }
    }
}
