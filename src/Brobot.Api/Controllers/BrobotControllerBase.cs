using Brobot.Api.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Controllers
{
    public abstract class BrobotControllerBase : ControllerBase
    {
        protected BrobotDbContext Context { get; }

        public BrobotControllerBase(BrobotDbContext context)
        {
            Context = context;
        }

        public ActionResult InternalServerError(string message, Exception exception = null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = message, exception = exception?.Message ?? string.Empty });
        }
    }
}
