using QuakeLogger.Domain.Interfaces.Repositories;
using QuakeLogger.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuakeLogger.Services
{
    public class ReportPrinter
    {
        private readonly IQuakeGameRepo _repoG;
        private readonly IQuakePlayerRepo _repoP;

        public ReportPrinter(IQuakeGameRepo repositoryG, IQuakePlayerRepo repositoryP)
        {
            _repoG = repositoryG;
            _repoP = repositoryP;
        }

        public void Print()
        {
            List<Game> games = _repoG.GetAll();            
            
            foreach (Game game in games)
            {
                int gameId = game.Id;
                Console.WriteLine("Game Id: " + gameId + "\n");

                foreach (Player player in game.GamePlayers.Where(i => i.GameId == gameId).Select(p => p.Player))
                {
                    if (player.Name == "<world>")
                        continue;

                    Console.WriteLine("Player name: " + player.Name + " ---- Kills: " + player.PlayerGames.Where(i => i.GameId == gameId).Select(k => k.Kills).Sum());                    
                }

                Console.WriteLine();
            }
            
        }
    }
}
