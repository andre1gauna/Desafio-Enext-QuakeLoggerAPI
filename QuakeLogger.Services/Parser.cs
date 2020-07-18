using Microsoft.EntityFrameworkCore.Internal;
using QuakeLogger.Domain.Interfaces.Repositories;
using QuakeLogger.Domain.Models;
using QuakeLogger.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuakeLogger.Services
{
    public class Parser
    {

        private readonly IQuakeGameRepo _repoG;
        private readonly IQuakePlayerRepo _repoP;
        private int actualGameId = 0;
        public Parser(IQuakeGameRepo repositoryG, IQuakePlayerRepo repositoryP)
        {
            _repoG = repositoryG;
            _repoP = repositoryP;
        }

        public void Reader(string Filename)
        {
            StreamReader reader = File.OpenText(Filename);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(' ');
                LineChecker(items);
            }
        }
        private string FindKilled(string[] items)
        {
            string killed = "";
            int index = items.IndexOf("killed");
            index++;
            
            while ((items[index])!="by")
            {
                killed += items[index]+ " ";
                index++;
            }
            
            return killed.Trim();
        }
        private string FindKiller(string[] items)
        {
            string killer = "";
            int index = items.IndexOf("killed");
            index--;
            Regex reg = new Regex(@"\d:");

            while (!reg.IsMatch(items[index]))
            {
                killer += items[index] + " ";
                index--;
            }

            killer = ReverseString(killer);            
            return killer;
        }

        private void AddPlayer(string player, bool isKiller)
        {
            int kill = 0;
            if (isKiller)
                kill = 1;

            Player createPlayer = new Player { Name = player };
            createPlayer.PlayerGames = new List<GamePlayer>
                { new GamePlayer  {Player = createPlayer,
                                  PlayerId = createPlayer.Id,
                                  Game = _repoG.FindById(actualGameId),
                                  GameId= actualGameId,
                                  Kills=kill                                  
                                  }
                };

                _repoP.Update(createPlayer);                          
            
        }
        private void AddPlayerTogame(string player, bool isKiller)
        {            
            Player playerToBeUpdated = _repoP.FindByName(player);
            int kill = 0;
            if (isKiller)
                kill = 1;

            playerToBeUpdated.PlayerGames.Add(
                new GamePlayer
                {
                    Player = playerToBeUpdated,
                    PlayerId = playerToBeUpdated.Id,
                    Game = _repoG.FindById(actualGameId),
                    GameId = actualGameId,
                    Kills = kill
                });

                _repoP.Update(playerToBeUpdated);                            
        }

        private void AddKill(string player)
        {
                Player playerToBeUpdated = _repoP.FindByName(player);
                playerToBeUpdated.PlayerGames
                       .Where(i => i.GameId == actualGameId)
                       .FirstOrDefault().Kills++;
                _repoP.Update(playerToBeUpdated);           
            
        }       

        private void LineChecker(string[] line)
        {
            string killer;
            string killed;

            if (line.Contains("InitGame:"))
            {
                CreateGame();
            }

            else if (line.Contains("Kill:"))
            {
                killer = FindKiller(line);
                killed = FindKilled(line);

                if (_repoP.FindByName(killer) == null)
                {
                    AddPlayer(killer, true);                    
                }
                else if (!HasGame(killer))
                    AddPlayerTogame(killer, true);

                else
                    AddKill(killer);

                if (_repoP.FindByName(killed) == null)
                {                    
                    AddPlayer(killed, false);
                }
                else if (!HasGame(killed))
                    AddPlayerTogame(killed, false);               

            }

            else if (line.Contains("ShutdownGame:"))
            {
                Game gameTobeClosed =_repoG.FindById(actualGameId);                
                IQueryable<GamePlayer> GamePlayersByGameId_IQueryable = _repoP.FindByGameId(actualGameId);
                ICollection<GamePlayer> GamePlayersByGameId_List = new List<GamePlayer>();  // trecho necessário para converter de IQueryable para List
                foreach ( GamePlayer gamePlayer in GamePlayersByGameId_IQueryable)          //
                {                                                                           //
                    GamePlayersByGameId_List.Add(gamePlayer);                               //
                }                                                                           //
                
                gameTobeClosed.GamePlayers = GamePlayersByGameId_List;                
                gameTobeClosed.TotalKills = GamePlayersByGameId_List.Select(k => k.Kills).Sum();


                _repoG.Update(gameTobeClosed);

                //if (gameTobeClosed.Id == 2)
                //{
                //   var g = _repoG.FindById(2);

                //}
            }

        }
        private void CreateGame()
        {
            actualGameId = _repoG.Add(new Game());
        }
        private string ReverseString(string toBeReversed)
        {
            string result = "";
            string[] intermediateString = toBeReversed.Split(" ");
            for (int i = intermediateString.Length; i != 0; i--)
            {
                result += " " + intermediateString[i - 1];
            }
            return result.Trim();
        }

        private bool HasGame(string player)
        {
            return _repoP
                .FindByName(player)
                .PlayerGames
                .Select(i => i.GameId)
                .Contains(actualGameId);
        }
    }
}
