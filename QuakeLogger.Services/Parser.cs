using Microsoft.EntityFrameworkCore.Internal;
using QuakeLogger.Domain.Interfaces.Repositories;
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
            string[] killer = new string[items.Length];
            int index = items.IndexOf("killed");
            index--;
            Regex reg = new Regex(@"\d:");
            while (!reg.IsMatch(items[index]))
            {
                killer.Append(items[index]);
                index--;
            }

            return killer.ToString().Trim();
        }

        private string FindKilled(string[] items)
        {
            string[] killed = new string[items.Length];
            int index = items.IndexOf("killed");
            index++;
            while (items[index] != "by")
            {
                killed.Append(items[index]);
                index++;
            }

            killed.Reverse();
            return killed.ToString().Trim();
        }

        private void AddPlayer(string player)
        {
            if (_repoP.FindByName(player) == null)
                _repoP.Add(new Player { Name = player});          

        }
        
        private void CheckKill (string player, string killerOrKilled)
        {
            if (killerOrKilled == "killer")
                _repoP.FindByName(player).Kills++;

            else if (killerOrKilled == "killed")
                _repoP.FindByName(player).Kills--;
        }

        private void CreateGame()
        {
            Game game = new Game();
            _repoP.GetAll().Where(x => x.GameId == 0).Select(x => x.GameId = game.Id);
            game.Players = _repoP.GetAll().Where(i => i.GameId == game.Id);
            _repoG.Add(game);
        }

        private void LineChecker(string[] items)
        {
            string killer;
            string killed;

            if (items.Contains("Kill:"))
            {
                killer = FindKiller(items);
                killed = FindKilled(items);
                AddPlayer(killer);
                AddPlayer(killed);
                CheckKill(killer, "killer");
                CheckKill(killed, "killed");
            }
            else if (items.Contains("ShutdownGame:"))
            {
                CreateGame();
            }
        }
    }
}
