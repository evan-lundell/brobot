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
using Microsoft.EntityFrameworkCore;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Reminder>>> GetReminders([FromQuery]ulong? channelId, [FromQuery]DateTime? reminderDateUtc)
        {
            try
            {
                var remindersQuery = Context.Reminders
                    .AsNoTracking()
                    .AsQueryable();

                if (channelId.HasValue)
                {
                    remindersQuery = remindersQuery.Where(r => r.ChannelId == channelId.Value);
                }

                if (reminderDateUtc.HasValue)
                {
                    remindersQuery = remindersQuery.Where(r => r.ReminderDateUtc == reminderDateUtc.Value);
                }

                var reminders = await remindersQuery.ToListAsync();
                return Ok(Mapper.Map<IEnumerable<Entities.Reminder>, IEnumerable<Models.Reminder>>(reminders));
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to get reminders", ex);
            }
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

        [HttpPut("{id}")]
        public async Task<ActionResult<Models.Reminder>> UpdateReminder(int id, [FromBody]Models.Reminder reminder)
        {
            try
            {
                var existingReminder = await Context.Reminders.FindAsync(id);
                if (existingReminder == null)
                {
                    return NotFound();
                }

                existingReminder.ChannelId = reminder.ChannelId;
                existingReminder.Message = reminder.Message;
                existingReminder.OwnerId = reminder.OwnerId;
                existingReminder.SentDateUtc = reminder.SentDateUtc;
                existingReminder.ReminderDateUtc = reminder.ReminderDateUtc;

                await Context.SaveChangesAsync();

                return Ok(Mapper.Map<Entities.Reminder, Models.Reminder>(existingReminder));
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to update reminder", ex);
            }
        }
    }
}
