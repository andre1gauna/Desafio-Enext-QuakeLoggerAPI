using AutoMapper;
using QuakeLogger.API.ViewModels;
using QuakeLogger.Models;
using QuakeLogger.ViewModels;
using System;
using System.Linq;

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

            CreateMap<Player, PlayerViewModel>()                 
                 .ForMember(dest => dest.Kills, opt => opt.MapFrom(src => src.PlayerGames.Where(i => i.PlayerId == src.Id).Select(k => k.Kills).Sum()));

            CreateMap<Game, GameViewModel>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.GamePlayers.Where(i => i.GameId == src.Id).Select(p => p.Player)))
                 .ForMember(dest => dest.TotalKills, opt => opt.MapFrom(src => src.GamePlayers
                                                                                  .Where(i => i.GameId == src.Id)
                                                                                  .Select(x => Math.Abs(x.Kills))
                                                                                  .Sum()));
        }
    }
}

