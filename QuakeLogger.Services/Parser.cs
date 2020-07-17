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

        private string FindKiller(string[] items)
        {
            string killer = "";
            int index = items.IndexOf("killed");
            index--;
            Regex reg = new Regex(@"\d:");

            while (!reg.IsMatch(items[index]))
            {
                killer += " " + items[index];
                index--;
            }

            killer = ReverseString(killer);            
            return killer;
        }

        private string FindKilled(string[] items)
        {
            string killed = "";
            int index = items.IndexOf("killed");
            index++;
            while (items[index] != "by")
            {
                killed += items[index] + " ";
                index++;
            }
            return killed.Trim();
        }

        private void AddPlayerAndCheckKill(string player, string killerOrKilled)
        {
            if (_repoP.FindByName(player) == null)
            {
                var createPlayer = new Player { Name = player };
                createPlayer.PlayerGames = new List<GamePlayer>
                { new GamePlayer  {Player = createPlayer,
                                  PlayerId = createPlayer.Id,
                                  Game = _repoG.FindById(actualGameId),
                                  GameId= actualGameId
                                  }

                };
                _repoP.Update(createPlayer);
                CheckKill(player, killerOrKilled);
            }
            else if (!HasGame(player))
            {
                var playerToBeUpdated = _repoP.FindByName(player);

                playerToBeUpdated.PlayerGames.Add(
                    new GamePlayer
                    {
                        Player = playerToBeUpdated,
                        PlayerId = playerToBeUpdated.Id,
                        Game = _repoG.FindById(actualGameId),
                        GameId = actualGameId

                    });
                _repoP.Update(playerToBeUpdated);
                CheckKill(player, killerOrKilled);
            }            
            else
                CheckKill(player, killerOrKilled);
        }

        private void CheckKill(string player, string killerOrKilled)
        {
            if (killerOrKilled == "killer")
            {
                var playerToBeUpdated = _repoP.FindByName(player);
                playerToBeUpdated.PlayerGames
                       .Where(i => i.GameId == actualGameId)
                       .FirstOrDefault().Kills++;

                _repoP.Update(playerToBeUpdated);

            }

            else if (killerOrKilled == "killed")
            {
                var playerToBeUpdated = _repoP.FindByName(player);
                playerToBeUpdated.PlayerGames
                       .Where(i => i.GameId == actualGameId)
                       .FirstOrDefault().Kills--;
                _repoP.Update(playerToBeUpdated);
            }
        }       

        private void LineChecker(string[] line)
        {
            string killer;
            string killed;
            int kills=0;
            if (line.Contains("InitGame:"))
            {
                CreateGame();
            }

            else if (line.Contains("Kill:"))
            {
                killer = FindKiller(line);
                killed = FindKilled(line);
                AddPlayerAndCheckKill(killer, "killer");
                AddPlayerAndCheckKill(killed, "killed");
            }
            else if (line.Contains("ShutdownGame:"))
            {
                Game gameTobeClosed =_repoG.FindById(actualGameId);                
                var ar = _repoP.FindByGameId(actualGameId);
                ICollection<GamePlayer> teste = new List<GamePlayer>();
                foreach ( var a in ar)
                {
                    teste.Add(a); 
                }
                if (actualGameId ==7)
                {
                    int acd;

                }
                gameTobeClosed.GamePlayers = teste;
                foreach(int kill in gameTobeClosed.GamePlayers.Select(x => x.Kills))
                {
                    kills += kill;
                }
                gameTobeClosed.TotalKills = kills;


                _repoG.Update(gameTobeClosed);
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
