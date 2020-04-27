using AutoMapper;
using Brobot.Api.Contexts;
using Entities = Brobot.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models = Brobot.Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace Brobot.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServersController : BrobotControllerBase
    {
        public ServersController(BrobotDbContext context, ILogger<ServersController> logger, IMapper mapper)
            : base(context, logger, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Server>>> Get()
        {
            try
            {
                var serverEntities = await Context.Servers
                    .Include(s => s.Channels)
                    .ThenInclude(c => c.DiscordUserChannels)
                    .ThenInclude(duc => duc.DiscordUser)
                    .ToListAsync();

                var serverModels = Mapper.Map<IEnumerable<Models.Server>>(serverEntities);
                return Ok(serverModels);
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to get servers", ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Server>> Get(ulong id)
        {
            try
            {
                var serverEntity = await Context.Servers.FindAsync(id);
                if (serverEntity == null)
                {
                    return NotFound();
                }

                return Ok(Mapper.Map<Models.Server>(serverEntity));
            }
            catch (Exception ex)
            {
                return InternalServerError($"Failed to server {id}", ex);
            }
        }

        [HttpPost("sync")]
        [Authorize(Roles = "Sync")]
        public async Task<ActionResult> Sync([FromBody]IEnumerable<Models.Server> servers)
        {
            try
            {
                var existingServers = await Context.Servers
                    .Include(s => s.Channels)
                    .ThenInclude(c => c.DiscordUserChannels)
                    .ThenInclude(duc => duc.DiscordUser)
                    .ToListAsync();

                var serversToDelete = existingServers.Where(es => !servers.Any(s => s.ServerId == es.ServerId));
                var channelsToDelete = new List<Entities.Channel>();
                var discordUserChannelsToDelete = new List<Entities.DiscordUserChannel>();

                var serversToAdd = new List<Entities.Server>();
                var channelsToAdd = new List<Entities.Channel>();
                var discordUsersToAdd = new List<Entities.DiscordUser>();

                foreach (var server in servers)
                {
                    var existingServer = existingServers.FirstOrDefault(es => es.ServerId == server.ServerId);
                    if (existingServer == null)
                    {
                        serversToAdd.Add(Mapper.Map<Entities.Server>(server));
                        continue;
                    }

                    existingServer.Name = server.Name;
                    channelsToDelete.AddRange(existingServer.Channels.Where(ec => !server.Channels.Any(c => c.ChannelId == ec.ChannelId)));

                    foreach (var channel in server.Channels)
                    {
                        var existingChannel = existingServer.Channels.FirstOrDefault(c => c.ChannelId == channel.ChannelId);
                        if (existingChannel == null)
                        {
                            channelsToAdd.Add(Mapper.Map<Entities.Channel>(channel));
                            continue;
                        }

                        existingChannel.Name = channel.Name;
                        discordUserChannelsToDelete.AddRange(existingChannel.DiscordUserChannels.Where(duc => !channel.DiscordUsers.Any(du => du.DiscordUserId == duc.DiscordUserId)));
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to sync.", ex);
            }
        }
    }
}
