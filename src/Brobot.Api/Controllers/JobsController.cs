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
    public class JobsController : BrobotControllerBase
    {
        public JobsController(BrobotDbContext context, ILogger<JobsController> logger, IMapper mapper) 
            : base(context, logger, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Job>>> GetJobs()
        {
            try
            {
                var jobs = await Context.Jobs
                    .AsNoTracking()
                    .Include(j => j.JobParameters)
                    .ThenInclude(jp => jp.JobParameterDefinition)
                    .Include(j => j.JobDefinition)
                    .ThenInclude(jd => jd.JobParameterDefinitions)
                    .Include(j => j.JobChannels)
                    .ThenInclude(jc => jc.Channel)
                    .ToListAsync();

                return Ok(Mapper.Map<IEnumerable<Entities.Job>, IEnumerable<Models.Job>>(jobs));
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to get jobs.", ex);
            }
        }
    }
}
