using Microsoft.EntityFrameworkCore.Internal;
using QuakeLogger.Domain.Interfaces.Repositories;
using QuakeLogger.Domain.Models;
using QuakeLogger.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace QuakeLogger.Services
{
    public class Parser
    {
        private readonly IQuakeGameRepo _repoG;
        private readonly IQuakePlayerRepo _repoP;
        private readonly IQuakeKillMethodRepo _repoKM;

        public Parser(IQuakeGameRepo repositoryG, IQuakePlayerRepo repositoryP, IQuakeKillMethodRepo repositoryKM)
        {
            _repoG = repositoryG;
            _repoP = repositoryP;
            _repoKM = repositoryKM;
        }

        public Game GetGame(int id)
        {
            return _repoG.FindById(id);
        }

        public List<Player> GetPlayersByGameId (int gameId)
        {
            List<Player> players = _repoP.GetAll();
            List <Player> resultPlayers = new List<Player>();
            foreach (Player player in players)
            {
                if (!(player.PlayerGames.Where(i => i.GameId == gameId) == null))
                {
                    resultPlayers.Add(player);
                }
            }
            return resultPlayers;
        }

        public List<KillMethod> GetKillMethodsByGameId (int gameId)
        {
            List<KillMethod> KillMethods = _repoKM.GetAll();
            List<KillMethod> resultKillMethods = new List<KillMethod>();
            foreach (KillMethod KillMethod in KillMethods)
            {
                if (KillMethod.GameId==gameId)
                {
                    resultKillMethods.Add(KillMethod);
                }
            }
            return resultKillMethods;
        }


        public void Reader(string Filename)
        {
            (int, List<int>) actualGameIdAndActualKillMethods = (0, new List<int>());            
            StreamReader reader = File.OpenText(Filename);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(' ');
                actualGameIdAndActualKillMethods = LineChecker(items, actualGameIdAndActualKillMethods.Item1, actualGameIdAndActualKillMethods.Item2);
            }
        }
        private (int, List<int>) LineChecker(string[] line, int actualGameId, List<int> KillMethods) 
        {
            string killer;
            string killed;
            string killMethod;            

            if (line.Contains("InitGame:"))
            {
                actualGameId = CreateGame();
            }

            else if (line.Contains("Kill:"))
            {
                killer = FindKiller(line);
                killed = FindKilled(line);
                killMethod = GetKillMethod(line);
                AddKillMethod(killMethod, actualGameId);

                if (_repoP.FindByName(killer) == null)
                {
                    AddPlayer(killer, true, actualGameId);
                }
                else if (!HasGame(killer, actualGameId))
                    AddPlayerTogame(killer, true, actualGameId);
                else
                    AddKill(killer, true, actualGameId);

                if (_repoP.FindByName(killed) == null)
                {
                    AddPlayer(killed, false, actualGameId);
                }
                else if (!HasGame(killed, actualGameId))
                    AddPlayerTogame(killed, false, actualGameId);
                else
                    AddKill(killed, false, actualGameId);

            }

            else if (line.Contains("ShutdownGame:"))
            {
                Game gameToBeClosed = _repoG.FindById(actualGameId);
                IQueryable<GamePlayer> GamePlayersByGameId_IQueryable = _repoP.FindByGameId(actualGameId);
                ICollection<GamePlayer> GamePlayersByGameId_List = new List<GamePlayer>();  // trecho necessário para converter de IQueryable para List
                foreach (GamePlayer gamePlayer in GamePlayersByGameId_IQueryable)          //
                {                                                                           //
                    GamePlayersByGameId_List.Add(gamePlayer);                               //
                }                                                                           //


                gameToBeClosed.KillMethods = _repoKM.GetAll()
                                                    .Where(i => i.GameId == actualGameId)
                                                    .ToList();              
                gameToBeClosed.GamePlayers = GamePlayersByGameId_List;
                gameToBeClosed.TotalKills = GamePlayersByGameId_List.Select(k => k.Kills).Sum();
                
                _repoG.Update(gameToBeClosed);
                
            }
           
            (int, List<int>) actualGameIdAndActualKillMethods = (actualGameId, KillMethods);
            return actualGameIdAndActualKillMethods;
        }
        private int CreateGame()
        {
            return _repoG.Add(new Game { KillMethods = new List<KillMethod>() });
        }
        private bool HasGame(string player, int actualGameId)
        {
            return _repoP
                .FindByName(player)
                .PlayerGames
                .Select(i => i.GameId)
                .Contains(actualGameId);
        }
        private void AddPlayer(string player, bool isKiller, int actualGameId)
        {

            Player createPlayer = new Player { Name = player };
            createPlayer.PlayerGames = new List<GamePlayer>
                { new GamePlayer  {Player = createPlayer,
                                  PlayerId = createPlayer.Id,
                                  Game = _repoG.FindById(actualGameId),
                                  GameId= actualGameId,
                                  }
                };

            _repoP.Add(createPlayer);
            AddKill(player, isKiller, actualGameId);
        }

        private void AddPlayerTogame(string player, bool isKiller, int actualGameId)
        {
            Player playerToBeUpdated = _repoP.FindByName(player);

            playerToBeUpdated.PlayerGames.Add(
                new GamePlayer
                {
                    Player = playerToBeUpdated,
                    PlayerId = playerToBeUpdated.Id,
                    Game = _repoG.FindById(actualGameId),
                    GameId = actualGameId,
                });

            _repoP.Update(playerToBeUpdated);
            AddKill(player, isKiller, actualGameId);
        }

        private void AddKill(string player, bool isKiller, int actualGameId)
        {
            Player playerToBeUpdated = _repoP.FindByName(player);

            if (isKiller)
            {
                playerToBeUpdated.PlayerGames
                         .Where(i => i.GameId == actualGameId)
                         .FirstOrDefault().Kills++;
            }

            else
            {
                playerToBeUpdated.PlayerGames
                         .Where(i => i.GameId == actualGameId)
                         .FirstOrDefault().Kills--;
            }

            _repoP.Update(playerToBeUpdated);

        }
        private string GetKillMethod(string[] line)
        {
            int index = line.IndexOf("killed");
            while ((line[index]) != "by")
                index++;

            index++;

            return line[index];
        }
        private void AddKillMethod(string killMethod, int actualGameId)
        {
            KillMethod KM = _repoG.FindById(actualGameId).KillMethods.Find(k => k.NameId == killMethod);
            if ( KM == null)
            {             
               _repoKM.Add(new KillMethod {NameId = killMethod, 
                                                    Count = 1, 
                                                    Game = _repoG.FindById(actualGameId), 
                                                    GameId = actualGameId });               
            }
            else
            {                
                KM.Count++;
                _repoKM.Update(KM);                
            }           
            
        }
        private string FindKilled(string[] line)
        {
            string killed = "";

            int index = line.IndexOf("killed");
            index++;

            while ((line[index]) != "by")
            {
                killed += line[index] + " ";
                index++;
            }

            return killed;
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
        

    }
}
