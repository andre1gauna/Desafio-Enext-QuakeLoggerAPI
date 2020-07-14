using AutoMapper;
using AutoMapper.Internal;
using QuakeLogger.API.ViewModels;
using QuakeLogger.Models;
using QuakeLogger.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuakeLogger.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public static MapperConfiguration RegisterMappings()
        {
            return new MapperConfiguration(x => x.AllowNullCollections = true);
        }

        public AutoMapperConfig()
        {
            CreateMap<Player, PlayerViewModel>().ReverseMap();
            CreateMap<Game, GameViewModel<PlayerViewModel>>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players))
                .ForMember(dest => dest.TotalKills, opt => opt.MapFrom(src => src.TotalKills));
        }
    }
}

