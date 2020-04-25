using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthTestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok("good");
        }

        [HttpGet("Authenticated")]
        [Authorize]
        public ActionResult<string> Authenticated()
        {
            return Ok("good");
        }

        [HttpGet("Authorized")]
        [Authorize(Roles = "Blah")]
        public ActionResult<string> Authorized()
        {
            return Ok("good");
        }
    }
}
