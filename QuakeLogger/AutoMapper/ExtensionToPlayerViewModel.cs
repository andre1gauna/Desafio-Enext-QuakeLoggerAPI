using QuakeLogger.API.ViewModels;
using QuakeLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuakeLogger.API.AutoMapper
{
    public static class ExtensionToPlayerViewModel
    {
        public static PlayerViewModel ToPlayerViewModel(this Player player)
        {
            return new PlayerViewModel
            {
                Id = player.Id,
                Name = player.Name,
                Kills = player.PlayerGames.Where(i => i.PlayerId == player.Id).Select(k => k.Kills).Sum(),
                GamesId = player.PlayerGames.Where(i => i.PlayerId == player.Id).Select(gI => gI.GameId)
            };
        }
    }
  
}
