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

        public Parser(IQuakeGameRepo repositoryG, IQuakePlayerRepo repositoryP)
        {
            _repoG = repositoryG;
            _repoP = repositoryP;
        }

        public void Reader(string Filename)
        {
            int actualGameId = 0;
            StreamReader reader = File.OpenText(Filename);
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                string[] items = line.Split(' ');
                actualGameId = LineChecker(items, actualGameId);
            }
        }
        private string FindKilled(Game game, string[] line)
        {
            (string, string) tuple = FindKilledAndKillMethod(line);
            
            return tuple.Item1.Trim();
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

            _repoP.Update(createPlayer);
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

        private int LineChecker(string[] line, int actualGameId)
        {
            Game game = new Game();
            game.KillMethods = new List<KillMethod>();
            string killer;
            string killed;

            if (line.Contains("InitGame:"))
            {
                actualGameId = CreateGame();                
            }

            else if (line.Contains("Kill:"))
            {
                killer = FindKiller(line);
                killed = FindKilled(game, line);
                (string, string) tuple = FindKilledAndKillMethod(line); // Necessário para encontrar o KillMethod atual.
                
                game = getKillMethod(game, tuple.Item2); // O segundo item é o KillMethod.

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
                gameToBeClosed.KillMethods = game.KillMethods;
                IQueryable<GamePlayer> GamePlayersByGameId_IQueryable = _repoP.FindByGameId(actualGameId);
                ICollection<GamePlayer> GamePlayersByGameId_List = new List<GamePlayer>();  // trecho necessário para converter de IQueryable para List
                foreach (GamePlayer gamePlayer in GamePlayersByGameId_IQueryable)          //
                {                                                                           //
                    GamePlayersByGameId_List.Add(gamePlayer);                               //
                }                                                                           //

                gameToBeClosed.GamePlayers = GamePlayersByGameId_List;
                gameToBeClosed.TotalKills = GamePlayersByGameId_List.Select(k => k.Kills).Sum();
                _repoG.Update(gameToBeClosed);
            }
            return actualGameId;
        }
        private int CreateGame()
        {
            return _repoG.Add(new Game());
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

        private bool HasGame(string player, int actualGameId)
        {
            return _repoP
                .FindByName(player)
                .PlayerGames
                .Select(i => i.GameId)
                .Contains(actualGameId);
        }

        private Game getKillMethod(Game game, string killMethod)
        {
            KillMethod KillMethod = new KillMethod { NameId = killMethod, Count = 1 };
            if (!game.KillMethods.Contains(KillMethod))
                game.KillMethods.Add(KillMethod);
            else
                game.KillMethods.Find(k => k.NameId == killMethod).Count++;

            return game;
        }

        private (string, string) FindKilledAndKillMethod(string[] line)
        {
            string killed = "";            

            int index = line.IndexOf("killed");
            index++;

            while ((line[index]) != "by")
            {
                killed += line[index] + " ";
                index++;
            }
            index++;
            (string, string) tuple = (killed, line[index]);
            return tuple;

        }
    }
}
