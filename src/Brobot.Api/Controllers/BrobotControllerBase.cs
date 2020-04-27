using AutoMapper;
using Brobot.Api.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Controllers
{
    public abstract class BrobotControllerBase : ControllerBase
    {
        protected BrobotDbContext Context { get; }
        protected ILogger Logger { get; }
        protected IMapper Mapper { get; }

        public BrobotControllerBase(BrobotDbContext context, ILogger logger, IMapper mapper)
        {
            Context = context;
            Logger = logger;
            Mapper = mapper;
        }

        public ActionResult InternalServerError(string message, Exception exception)
        {
            Logger.LogError(exception, message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = message, exception = exception.Message });
        }
    }
}
