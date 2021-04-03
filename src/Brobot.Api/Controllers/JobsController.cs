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
                    .ThenInclude(c => c.DiscordUserChannels)
                    .ThenInclude(duc => duc.DiscordUser)
                    .ToListAsync();

                return Ok(Mapper.Map<IEnumerable<Entities.Job>, IEnumerable<Models.Job>>(jobs));
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to get jobs.", ex);
            }
        }

        [HttpPut("{jobId}/parameter/{jobParameterId}")]
        public async Task<ActionResult<Models.JobParameter>> UpdateParameter(int jobId, int jobParameterId, [FromBody]Models.JobParameter jobParameter)
        {
            try
            {
                var jobEntity = await Context.Jobs
                    .Include(j => j.JobParameters)
                    .SingleOrDefaultAsync(j => j.JobId == jobId);

                if (jobEntity == null)
                {
                    return NotFound($"Job {jobId} not found");
                }

                var parameterEntity = jobEntity.JobParameters.FirstOrDefault(jp => jp.JobParameterId == jobParameterId);
                if (parameterEntity == null)
                {
                    return NotFound($"Job Parameter {jobParameterId} not found");
                }

                parameterEntity.Value = jobParameter.Value;
                await Context.SaveChangesAsync();

                return Ok(jobParameter);
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to update job parameter", ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Job>> GetJob(int id)
        {
            try
            {
                var jobEntity = await Context.Jobs
                    .AsNoTracking()
                    .OrderBy(j => j.JobId)
                    .Include(j => j.JobParameters)
                    .ThenInclude(jp => jp.JobParameterDefinition)
                    .Include(j => j.JobDefinition)
                    .ThenInclude(jd => jd.JobParameterDefinitions)
                    .Include(j => j.JobChannels)
                    .ThenInclude(jc => jc.Channel)
                    .ThenInclude(c => c.DiscordUserChannels)
                    .ThenInclude (duc => duc.DiscordUser)
                    .SingleOrDefaultAsync(j => j.JobId == id);
                
                if (jobEntity == null)
                {
                    return NotFound($"Job {id} not found");
                }

                return Ok(Mapper.Map<Entities.Job, Models.Job>(jobEntity));
            }
            catch (Exception ex)
            {
                return InternalServerError($"Failed to get job {id}", ex);
            }
        }
    }
}
