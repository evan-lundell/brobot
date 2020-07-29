using AutoMapper;
using Brobot.Api.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class RemindersController : BrobotControllerBase
    {
        public RemindersController(BrobotDbContext context, ILogger<RemindersController> logger, IMapper mapper) 
            : base(context, logger, mapper)
        {
        }

        [HttpPost]
        public async Task<ActionResult<Models.Reminder>> Post([FromBody]Models.Reminder reminder)
        {
            try
            {
                var reminderEntity = Mapper.Map<Models.Reminder, Entities.Reminder>(reminder);
                
                var discordUser = await Context.DiscordUsers.FindAsync(reminder.OwnerId);
                if (discordUser == null)
                {
                    return BadRequest(new { message = $"Unable to find user {reminder.OwnerId}" });
                }

                var channel = await Context.Channels.FindAsync(reminder.ChannelId);
                if (channel == null)
                {
                    return BadRequest(new { message = $"Unable to find channel {reminder.ChannelId}" });
                }

                if (!string.IsNullOrWhiteSpace(discordUser.Timezone))
                {
                    var timeZoneInfo = TZConvert.GetTimeZoneInfo(discordUser.Timezone);
                    reminderEntity.ReminderDateUtc = reminderEntity.ReminderDateUtc - timeZoneInfo.GetUtcOffset(DateTime.Now);
                }

                await Context.Reminders.AddAsync(reminderEntity);
                await Context.SaveChangesAsync();

                return reminder;
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to post reminder.", ex);
            }
        }
    }
}
