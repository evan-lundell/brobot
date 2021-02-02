using AutoMapper;
using Brobot.Api.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models = Brobot.Core.Models;
using Entities = Brobot.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Brobot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StopWordsController : BrobotControllerBase
    {
        public StopWordsController(BrobotDbContext context, ILogger<StopWordsController> logger, IMapper mapper) : 
            base(context, logger, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<Models.StopWord>> Get()
        {
            try
            {
                var words = await Context.StopWords
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(Mapper.Map<IEnumerable<Entities.StopWord>, IEnumerable<Models.StopWord>>(words));
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to get stop words", ex);
            }
        }
    }
}
