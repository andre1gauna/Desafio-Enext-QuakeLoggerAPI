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
            //Console.WriteLine(killer);
            return killer;
        }

        private string FindKilled(string[] items)
        {
            string killed="";
            int index = items.IndexOf("killed");
            index++;          
            while (items[index] != "by")
            {
                killed += items[index] + " ";
                index++;
            }
            
            //Console.WriteLine(killed);
            return killed.Trim();
        }

        private void AddPlayer(string player, string killerOrKilled)
        {
            if (_repoP.FindByName(player) == null)
            {
                _repoP.Add(new Player { Name = player });
                CheckKill(player, killerOrKilled);
            }

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
            int actualGameId = _repoG.Add(new Game());            
            var players = _repoP.GetAll().Where(x => x.GameId == 0);            
            if(actualGameId==2)
            {

            }
            foreach(Player player in players)
            {
                player.GamePlayers.Ga = _repoG.FindById(actualGameId);
                player.GameId = actualGameId;
                Console.WriteLine(player.GameId);
                _repoP.Update(player);
            }

            _repoG.FindById(actualGameId).Players = _repoP.GetAll().Where(x => x.GameId == actualGameId);            
        }

        private void LineChecker(string[] line)
        {
            string killer;
            string killed;

            if (line.Contains("Kill:"))
            {
                killer = FindKiller(line);
                killed = FindKilled(line);
                AddPlayer(killer, "killer");
                AddPlayer(killed, "killed");
                CheckKill(killer, "killer");
                CheckKill(killed, "killed");
            }
            else if (line.Contains("ShutdownGame:"))
            {
                CreateGame();
            }
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
