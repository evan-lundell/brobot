﻿using AutoMapper;
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
                    .AsNoTracking()
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
                var serverEntity = await Context.Servers
                    .Include(s => s.Channels)
                    .ThenInclude(c => c.DiscordUserChannels)
                    .ThenInclude(duc => duc.DiscordUser)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(s => s.ServerId == id);
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
                var userModels = servers
                    .SelectMany(s => s.Channels)
                    .SelectMany(c => c.DiscordUsers)
                    .Distinct()
                    .ToList();

                var existingServers = await Context.Servers
                    .Include(s => s.Channels)
                    .ThenInclude(c => c.DiscordUserChannels)
                    .ThenInclude(duc => duc.DiscordUser)
                    .Include(s => s.VoiceChannels)
                    .ToListAsync();

                var existingUsers = await Context.DiscordUsers
                    .Include(du => du .DiscordUserChannels)
                    .ToListAsync();

                var userEntities = new List<Entities.DiscordUser>();
                var newUsers = new List<Entities.DiscordUser>();
                Context.DiscordUsers.RemoveRange(existingUsers.Where(entity => !userModels.Any(model => model.DiscordUserId == entity.DiscordUserId)));
                foreach (var userModel in userModels)
                {
                    if (userEntities.Any(eu => eu.DiscordUserId == userModel.DiscordUserId))
                    {
                        continue;
                    }

                    var userEntity = existingUsers.FirstOrDefault(eu => eu.DiscordUserId == userModel.DiscordUserId);
                    if (userEntity == null)
                    {
                        userEntity = Mapper.Map<Entities.DiscordUser>(userModel);
                        newUsers.Add(userEntity);
                    }
                    else
                    {
                        userEntity.Username = userModel.Username;
                    }

                    userEntities.Add(userEntity);
                }

                await Context.DiscordUsers.AddRangeAsync(newUsers);
                Context.Servers.RemoveRange(existingServers.Where(entity => !servers.Any(model => model.ServerId == entity.ServerId)));
                foreach (var serverModel in servers)
                {
                    var serverEntity = existingServers.FirstOrDefault(entity => entity.ServerId == serverModel.ServerId);
                    
                    // new server
                    if (serverEntity == null)
                    {
                        serverEntity = Mapper.Map<Entities.Server>(serverModel);

                        foreach (var channelModel in serverModel.Channels)
                        {
                            var channelEntity = serverEntity.Channels.FirstOrDefault(c => c.ChannelId == channelModel.ChannelId);
                            if (channelEntity == null)
                            {
                                Logger.LogWarning($"Channel {channelModel.ChannelId} not found when creating server {serverModel.ServerId}");
                                continue;
                            }

                            foreach (var discordUserModel in channelModel.DiscordUsers)
                            {
                                var discordUserEntity = userEntities.FirstOrDefault(u => u.DiscordUserId == discordUserModel.DiscordUserId);
                                if (discordUserEntity == null)
                                {
                                    Logger.LogWarning($"Discord User {discordUserModel.DiscordUserId} not found when creating server {serverModel.ServerId}");
                                    continue;
                                }

                                channelEntity.DiscordUserChannels.Add(new Entities.DiscordUserChannel
                                {
                                    DiscordUser = discordUserEntity,
                                    DiscordUserId = discordUserEntity.DiscordUserId,
                                    Channel = channelEntity,
                                    ChannelId = channelEntity.ChannelId
                                });
                            }

                        }

                        await Context.Servers.AddAsync(serverEntity);
                        continue;
                    }

                    serverEntity.Name = serverModel.Name;

                    var removedChannels = serverEntity.Channels
                        .Where(entity => !serverModel.Channels.Any(model => model.ChannelId == entity.ChannelId))
                        .ToList();
                    foreach (var removedChannel in removedChannels)
                    {
                        serverEntity.Channels.Remove(removedChannel);
                    }

                    var removedVoiceChannels = serverEntity.VoiceChannels
                        .Where(entity => !serverModel.VoiceChannels.Any(model => model.Id == entity.Id))
                        .ToList();
                    foreach (var removedVoiceChannel in removedVoiceChannels)
                    {
                        serverEntity.VoiceChannels.Remove(removedVoiceChannel);
                    }

                    foreach (var channelModel in serverModel.Channels)
                    {
                        var channelEntity = serverEntity.Channels.FirstOrDefault(c => c.ChannelId == channelModel.ChannelId);
                        if (channelEntity == null)
                        {
                            channelEntity = Mapper.Map<Entities.Channel>(channelModel);
                            channelEntity.Server = serverEntity;
                            channelEntity.ServerId = serverEntity.ServerId;
                            await Context.Channels.AddAsync(channelEntity);
                        }
                        else
                        {
                            channelEntity.Name = channelModel.Name;
                            Context.DiscordUserChannels.RemoveRange(
                                channelEntity.DiscordUserChannels.Where(entity => !channelModel.DiscordUsers
                                .Any(model => entity.DiscordUserId == model.DiscordUserId)));
                        }
                        
                        foreach (var discordUserModel in channelModel.DiscordUsers)
                        {
                            if (!channelEntity.DiscordUserChannels.Any(duc => duc.DiscordUserId == discordUserModel.DiscordUserId))
                            {
                                var discordUserEntity = userEntities.FirstOrDefault(eu => eu.DiscordUserId == discordUserModel.DiscordUserId);
                                var discordUserChannel = new Entities.DiscordUserChannel
                                {
                                    Channel = channelEntity,
                                    ChannelId = channelEntity.ChannelId,
                                    DiscordUser = discordUserEntity,
                                    DiscordUserId = discordUserEntity?.DiscordUserId ?? 0
                                };
                                await Context.DiscordUserChannels.AddAsync(discordUserChannel);
                            }
                        }
                    }

                    foreach (var voiceChannelModel in serverModel.VoiceChannels)
                    {
                        var voiceChannelEntity = serverEntity.VoiceChannels.FirstOrDefault(vc => vc.Id == voiceChannelModel.Id);
                        if (voiceChannelEntity == null)
                        {
                            voiceChannelEntity = Mapper.Map<Models.VoiceChannel, Entities.VoiceChannel>(voiceChannelModel);
                            voiceChannelEntity.Server = serverEntity;
                            voiceChannelEntity.ServerId = serverEntity.ServerId;
                            await Context.VoiceChannels.AddAsync(voiceChannelEntity);
                        }
                        else
                        {
                            voiceChannelEntity.Name = voiceChannelModel.Name;
                        }
                    }
                }

                await Context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError("Failed to sync.", ex);
            }
        }
    }
}
 