using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuakeLogger.AutoMapper;
using QuakeLogger.Controllers;
using QuakeLogger.Data.Context;
using QuakeLogger.Data.Repositories;
using QuakeLogger.Domain.Models;
using QuakeLogger.Models;
using QuakeLogger.Services;
using QuakeLogger.Tests.Fakes;
using QuakeLogger.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace QuakeLogger.Tests.ServicesTests
{
    public class ParserTest
    {        
        private readonly ContextFake _contextFake;
        private Parser _parser;
        private QuakeLoggerContext _context;
        private QuakePlayerRepo _repoP;
        private QuakeGameRepo _repoG;
        private QuakeKillMethodRepo _repoKM;

        public ParserTest()
        {            
            _contextFake = new ContextFake("ParserTest");
            _context = _contextFake.GetContext("ParserTestingContext");
            _repoP = new QuakePlayerRepo(_context);
            _repoG = new QuakeGameRepo(_context);
            _repoKM = new QuakeKillMethodRepo(_context);
            _parser = new Parser(_repoG, _repoP, _repoKM);                    
        }

        [Fact]
        public void ReaderGettingGameById_ShouldWork()
        {
            Game actualGame = _parser.GetGame(1);
            Game expecetedGame = _context.Games.Where(x => x.Id == 1).FirstOrDefault();
            List<KillMethod> expectedKillMethods = _parser.GetKillMethodsByGameId(1);
            List<KillMethod> actualKillMethods = _context.KillMethods.Where(i => i.GameId == 1).ToList();
            Assert.Equal(expectedKillMethods, actualKillMethods);

            Assert.Equal(expecetedGame, actualGame);
        }

        [Fact]
        public void ReaderGettingPlayersByGameId_ShouldWork()
        {  
            List<Player> actualPlayers = _parser.GetPlayersByGameId(1);
            List<Player> allPlayers = _context.Players.ToList();
            List<Player> expectedPlayers = new List<Player>();

            foreach (Player player in allPlayers)
            {
                if (!(player.PlayerGames.Where(i => i.GameId == 1) == null))
                    expectedPlayers.Add(player);
            }

            Assert.Equal(expectedPlayers, actualPlayers);
        }

        [Fact]
        public void ReaderGettingKillMethodsByGameId_ShouldWork()
        {
            List<KillMethod> expectedKillMethods = _parser.GetKillMethodsByGameId(1);
            List<KillMethod> actualKillMethods = _context.KillMethods.Where(i => i.GameId == 1).ToList();
            Assert.Equal(expectedKillMethods, actualKillMethods);
        }        

    }
}
