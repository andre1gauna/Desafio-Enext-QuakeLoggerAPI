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
        private int actualGameId = 0;

        public Parser(IQuakeGameRepo repositoryG, IQuakePlayerRepo repositoryP, IQuakeKillMethodRepo repositoryKM)
        {
            _repoG = repositoryG;
            _repoP = repositoryP;
            _repoKM = repositoryKM;
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
        private void LineChecker(string[] line) 
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
                AddKillMethod(killMethod);

                if (_repoP.FindByName(killer) == null)
                {
                    AddPlayer(killer, true);
                }
                else if (!HasGame(killer))
                    AddPlayerTogame(killer, true);
                else
                    AddKill(killer, true);

                if (_repoP.FindByName(killed) == null)
                {
                    AddPlayer(killed, false);
                }
                else if (!HasGame(killed))
                    AddPlayerTogame(killed, false);
                else
                    AddKill(killed, false);

            }

            else if (line.Contains("ShutdownGame:"))
            {
                CloseGame();                
            }       
                
        }
        private int CreateGame()
        {
            return _repoG.Add(new Game { KillMethods = new List<KillMethod>() });
        }

        private void CloseGame()
        {
            Game gameToBeClosed = _repoG.FindById(actualGameId);
            IQueryable<GamePlayer> GamePlayersByGameId_IQueryable = _repoP.FindByGameId(actualGameId);
            ICollection<GamePlayer> GamePlayersByGameId_List = new List<GamePlayer>();  // trecho necessário para converter de IQueryable para List
            foreach (GamePlayer gamePlayer in GamePlayersByGameId_IQueryable)           //
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
        private bool HasGame(string player)
        {
            return _repoP
                .FindByName(player)
                .PlayerGames
                .Select(i => i.GameId)
                .Contains(actualGameId);
        }
        private void AddPlayer(string player, bool isKiller)
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
            AddKill(player, isKiller);
        }

        private void AddPlayerTogame(string player, bool isKiller)
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
            AddKill(player, isKiller);
        }

        private void AddKill(string player, bool isKiller)
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
        private void AddKillMethod(string killMethod)
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


        public Game GetGameById(int id)
        {
            return _repoG.FindById(id);
        }

        public List<Player> GetPlayersByGameId(int gameId)
        {
            List<Player> players = _repoP.GetAll();
            List<Player> resultPlayers = new List<Player>();
            foreach (Player player in players)
            {
                if (!(player.PlayerGames.Where(i => i.GameId == gameId) == null))
                {
                    resultPlayers.Add(player);
                }
            }
            return resultPlayers;
        }

        public List<KillMethod> GetKillMethodsByGameId(int gameId)
        {
            List<KillMethod> KillMethods = _repoKM.GetAll();
            List<KillMethod> resultKillMethods = new List<KillMethod>();
            foreach (KillMethod KillMethod in KillMethods)
            {
                if (KillMethod.GameId == gameId)
                {
                    resultKillMethods.Add(KillMethod);
                }
            }
            return resultKillMethods;
        }

    }
}
