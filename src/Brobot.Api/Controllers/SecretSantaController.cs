using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Brobot.Api.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models = Brobot.Core.Models;
using Entities = Brobot.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Brobot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecretSantaController : BrobotControllerBase
    {
        private readonly Random _random;

        public SecretSantaController(BrobotDbContext context, ILogger<SecretSantaController> logger, IMapper mapper, Random random) 
            : base(context, logger, mapper)
        {
            _random = random;
        }

        [HttpPost("group")]
        public async Task<ActionResult<Models.SecretSantaGroup>> CreateGroup([FromBody]Models.SecretSantaGroup group)
        {
            try
            {
                var existingGroup = await Context.SecretSantaGroups.AsNoTracking().SingleOrDefaultAsync(g => g.Name == group.Name);
                if (existingGroup != null)
                {
                    return BadRequest(new { message = $"Group {group.Name} already exists" });
                }

                var newGroup = Mapper.Map<Models.SecretSantaGroup, Entities.SecretSantaGroup>(group);
                await Context.SecretSantaGroups.AddAsync(newGroup);
                await Context.SaveChangesAsync();
                group.SecretSantaGroupId = newGroup.SecretSantaGroupId;

                return Ok(group);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Failed to create group {group?.Name ?? "<None>"}", ex);
            }
        }

        [HttpPost("group/{groupId}/member")]
        public async Task<ActionResult<Models.SecretSantaGroupDiscordUser>> AddMember(int groupId, [FromBody]Models.SecretSantaGroupDiscordUser member)
        {
            try
            {
                var group = await Context.SecretSantaGroups
                    .Include(ssg => ssg.SecretSantaGroupDiscordUsers)
                    .SingleOrDefaultAsync(g => g.SecretSantaGroupId == groupId);

                if (group == null)
                {
                    return NotFound(new { message = $"Group {groupId} does not exist." });
                }

                if (group.SecretSantaGroupDiscordUsers.Any(du => du.DiscordUserId == member.DiscordUserId && du.SecretSantaGroupId == member.SecretSantaGroupId))
                {
                    return BadRequest(new { message = $"User {member.DiscordUserId} already exists in group {groupId}" });
                }

                var newMember = Mapper.Map<Models.SecretSantaGroupDiscordUser, Entities.SecretSantaGroupDiscordUser>(member);
                group.SecretSantaGroupDiscordUsers.Add(newMember);
                await Context.SaveChangesAsync();

                return Ok(member);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Failed to add member to group {groupId}", ex);
            }
        }

        [HttpPost("group/{groupId}/event")]
        public async Task<ActionResult<Models.SecretSantaEvent>> CreateEvent(int groupId, [FromBody]Models.SecretSantaEvent secretSantaEvent)
        {
            try
            {
                var group = await Context.SecretSantaGroups
                    .Include(g => g.SecretSantaEvents)
                    .ThenInclude(sse => sse.SecretSantaPairings)
                    .Include(g => g.SecretSantaGroupDiscordUsers)
                    .ThenInclude(ssgdu => ssgdu.DiscordUser)
                    .SingleOrDefaultAsync(g => g.SecretSantaGroupId == groupId);

                if (group == null)
                {
                    return NotFound(new { message = $"Group {groupId} not found" });
                }

                if (group.SecretSantaEvents.Any(e => e.Year == secretSantaEvent.Year))
                {
                    return BadRequest(new { message = $"Group {groupId} already has an event associated to year {secretSantaEvent.Year}. Delete that event before creating a new one" });
                }

                var newEvent = Mapper.Map<Models.SecretSantaEvent, Entities.SecretSantaEvent>(secretSantaEvent);

                // Create pairings
                var lastYearPairings = new List<Entities.SecretSantaPairing>();
                var previousEvent = group.SecretSantaEvents.FirstOrDefault(sse => sse.Year == secretSantaEvent.Year - 1);
                if (previousEvent != null && group.CheckPastYearPairings.HasValue && group.CheckPastYearPairings.Value)
                {
                    lastYearPairings = previousEvent.SecretSantaPairings.ToList();
                }

                var givers = group.SecretSantaGroupDiscordUsers.ToList();
                var availableRecipients = group.SecretSantaGroupDiscordUsers.ToList();
                var pairings = new List<Entities.SecretSantaPairing>();
                while (givers.Count > 0)
                {
                    var giver = givers[0];
                    var previousYearRecipient = lastYearPairings.FirstOrDefault(p => p.GiverId == giver.DiscordUserId);
                    var validRecipients = availableRecipients.Where(r => r.DiscordUserId != giver.DiscordUserId && (previousYearRecipient == null || previousYearRecipient.RecipientId != r.DiscordUserId)).ToList();
                    if (validRecipients.Count == 0)
                    {
                        var swappable = pairings.Where(p => p.RecipientId != giver.DiscordUserId && (previousYearRecipient == null || previousYearRecipient.RecipientId != p.RecipientId)).ToList();
                        var swap = swappable[_random.Next(swappable.Count)];
                        var swapSecretSantaGroupDiscordUser = group.SecretSantaGroupDiscordUsers.FirstOrDefault(u => u.DiscordUserId == swap.GiverId);
                        givers.Add(swapSecretSantaGroupDiscordUser);
                        swap.Giver = giver.DiscordUser;
                        swap.GiverId = giver.DiscordUserId;
                    }
                    else
                    {
                        var recipient = validRecipients[_random.Next(validRecipients.Count)];
                        var pairing = new Entities.SecretSantaPairing
                        {
                            Giver = giver.DiscordUser,
                            GiverId = giver.DiscordUserId,
                            Recipient = recipient.DiscordUser,
                            RecipientId = recipient.DiscordUserId
                        };
                        pairings.Add(pairing);
                        availableRecipients.Remove(recipient);
                    }

                    givers.Remove(giver);
                }

                newEvent.SecretSantaPairings = pairings;
                group.SecretSantaEvents.Add(newEvent);
                await Context.SaveChangesAsync();

                secretSantaEvent.SecretSantaEventId = newEvent.SecretSantaEventId;
                secretSantaEvent.CreatedDateUtc = newEvent.CreatedDateUtc;
                foreach (var pair in pairings)
                {
                    secretSantaEvent.SecretSantaPairings.Add(Mapper.Map<Entities.SecretSantaPairing, Models.SecretSantaPairing>(pair));
                }

                return Ok(secretSantaEvent);
            }
            catch (Exception ex)
            {
                return InternalServerError($"Failed to create event for group {groupId}", ex);
            }
        }
    }
}
