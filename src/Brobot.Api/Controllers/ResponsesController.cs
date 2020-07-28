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
    public class ResponsesController : BrobotControllerBase
    {
        public ResponsesController(BrobotDbContext context, ILogger<ResponsesController> logger, IMapper mapper) 
            : base(context, logger, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.EventResponse>>> Get()
        {
            try
            {
                var responses = await Context.EventResponses
                    .Include(er => er.DiscordEvent)
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(Mapper.Map<IEnumerable<Entities.EventResponse>, IEnumerable<Models.EventResponse>>(responses));
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to get event responses.", ex);
            }
        }
    }
}
