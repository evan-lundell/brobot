using Brobot.Api.Contexts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brobot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServerController : BrobotControllerBase
    {
        public ServerController(BrobotDbContext context)
            : base(context)
        {
        }
    }
}
