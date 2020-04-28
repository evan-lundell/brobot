﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities = Brobot.Api.Entities;
using Models = Brobot.Core.Models;

namespace Brobot.Api.MappingProfiles
{
    public class EntityProfile : Profile
    {
        public EntityProfile()
        {
            CreateMap<Entities.Server, Models.Server>();
            CreateMap<Entities.Channel, Models.Channel>()
                .ForMember(entity => entity.DiscordUsers, opt => opt.MapFrom(model => model.DiscordUserChannels));

            CreateMap<Entities.DiscordUserChannel, Models.DiscordUser>()
                .ForMember(entity => entity.DiscordUserId, opt => opt.MapFrom(model => model.DiscordUserId))
                .ForMember(entity => entity.Username, opt => opt.MapFrom(model => model.DiscordUser.Username))
                .ForMember(entity => entity.Birthdate, opt => opt.MapFrom(model => model.DiscordUser.Birthdate))
                .ForMember(entity => entity.Timezone, opt => opt.MapFrom(model => model.DiscordUser.Timezone));

            CreateMap<Entities.DiscordUser, Models.DiscordUser>();

            CreateMap<Models.Server, Entities.Server>()
                .AfterMap((model, entity) =>
                {
                    foreach (var channel in entity.Channels)
                    {
                        channel.Server = entity;
                        channel.ServerId = entity.ServerId;
                    }
                });

            CreateMap<Models.Channel, Entities.Channel>()
                .ForMember(entity => entity.DiscordUserChannels, opt => opt.Ignore());

            CreateMap<Models.DiscordUser, Entities.DiscordUserChannel>()
                .ForMember(entity => entity.DiscordUser, opt => opt.MapFrom(model => model));

            CreateMap<Models.DiscordUser, Entities.DiscordUser>();
        }
    }
}
