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
using Microsoft.EntityFrameworkCore;

namespace Brobot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : BrobotControllerBase
    {
        public MessagesController(BrobotDbContext context, ILogger<MessagesController> logger, IMapper mapper)
            : base(context, logger, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.DailyMessageCount>>> Get([FromQuery]DateTime? startDate = null, [FromQuery]DateTime? endDate = null)
        {
            try
            {
                IQueryable<Entities.DailyMessageCount> query = Context.DailyMessageCounts
                    .AsNoTracking()
                    .Include(dmc => dmc.Channel)
                    .Include(dmc => dmc.DiscordUser);
                if (startDate.HasValue)
                {
                    query = query.Where(dmc => dmc.Day >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(dmc => dmc.Day <= endDate.Value);
                }

                var counts = await query.ToListAsync();
                return Ok(Mapper.Map<IEnumerable<Entities.DailyMessageCount>, IEnumerable<Models.DailyMessageCount>>(counts));
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to get daily message counts", ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<Models.DailyMessageCount>>> PostMessageCounts([FromBody]IEnumerable<Models.DailyMessageCount> dailyMessageCountModels)
        {
            try
            {
                var dailyMessageCountEntities = Mapper.Map<IEnumerable<Models.DailyMessageCount>, IEnumerable<Entities.DailyMessageCount>>(dailyMessageCountModels);
                await Context.DailyMessageCounts.AddRangeAsync(dailyMessageCountEntities);
                await Context.SaveChangesAsync();
                return Ok(Mapper.Map<IEnumerable<Entities.DailyMessageCount>, IEnumerable<Models.DailyMessageCount>>(dailyMessageCountEntities));
            }
            catch (Exception ex)
            { 
                return InternalServerError("Failed to create daily message counts", ex);
            }
        }
    }
}
