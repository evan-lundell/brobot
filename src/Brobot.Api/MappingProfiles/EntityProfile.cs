using AutoMapper;
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
            CreateMap<Entities.Server, Models.Server>().ReverseMap();
            CreateMap<Entities.Channel, Models.Channel>()
                .ForMember(du => du.DiscordUsers, opt => opt.MapFrom(c => c.DiscordUserChannels));

            CreateMap<Entities.DiscordUserChannel, Models.DiscordUser>()
                .ForMember(du => du.DiscordUserId, opt => opt.MapFrom(duc => duc.DiscordUserId))
                .ForMember(du => du.Username, opt => opt.MapFrom(duc => duc.DiscordUser.Username))
                .ForMember(du => du.Birthdate, opt => opt.MapFrom(duc => duc.DiscordUser.Birthdate))
                .ForMember(du => du.Timezone, opt => opt.MapFrom(duc => duc.DiscordUser.Timezone));

            CreateMap<Entities.DiscordUser, Models.DiscordUser>();

            CreateMap<Models.Channel, Entities.Channel>()
                .ForMember(e => e.DiscordUserChannels, opt => opt.MapFrom(c => c.DiscordUsers.Select(du => new { ChannelId = c.ChannelId, }))

            CreateMap<Models.DiscordUser, Entities.DiscordUserChannel>()
    
        }
    }
}
