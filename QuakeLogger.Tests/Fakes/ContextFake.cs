using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using QuakeLogger.Data.Context;
using QuakeLogger.Domain.Models;
using QuakeLogger.Models;
using System;
using System.Collections.Generic;

namespace QuakeLogger.Tests.Fakes
{
    public class ContextFake
    {
        private readonly string _prefixDataBase;

        public ContextFake(string prefixDataBase)
        {
            _prefixDataBase = prefixDataBase;
        }

        public QuakeLoggerContext GetContext(string dataBaseName)
        {
            var options = new DbContextOptionsBuilder<QuakeLoggerContext>()
                .UseInMemoryDatabase(_prefixDataBase + "_" + dataBaseName)
                .Options;
            return new QuakeLoggerContext(options);
        }
    }
    public static class ContextFakeExtensions
    {
        public static QuakeLoggerContext AddFakeGames(this QuakeLoggerContext context)
        {
            if (context.Games.Any()) return context;


            var Games = new List<Game>()
            {
                new Game() {Id = 1},
                new Game() {Id = 2}
            };

            context.Games.AddRange(Games);
            context.SaveChanges();

            foreach (var game in Games)
            {
                context.Entry<Game>(game).State = EntityState.Detached;
            }

            return context;
        }

        public static QuakeLoggerContext AddFakeKillMethods(this QuakeLoggerContext context)
        {
            if (context.KillMethods.Any()) return context;


            var KillMethods = new List<KillMethod>()
            {
                new KillMethod() {Id = 1, NameId = "MOD_TRIGGER_HURT", GameId =1},
                new KillMethod() {Id = 2, NameId = "MOD_ROCKET_SPLASH", GameId =1},                

                new KillMethod() {Id = 4, NameId = "MOD_TRIGGER_HURT",  GameId =2},
                new KillMethod() {Id = 5, NameId = "MOD_ROCKET_SPLASH", GameId =2}
                
            };

            context.KillMethods.AddRange(KillMethods);
            context.SaveChanges();

            foreach (var killMethod in KillMethods)
            {
                context.Entry<KillMethod>(killMethod).State = EntityState.Detached;                
            }

            return context;
        }

        public static QuakeLoggerContext AddFakePlayers(this QuakeLoggerContext context)
        {
            if (context.Players.Any()) return context;

            var Players = new List<Player>()
            {
                new Player() {Id = 1, Name = "<world>"},
                new Player() {Id = 2, Name = "Isgalamido "},
                new Player() {Id = 3, Name = "Mocinha  "},
            };

            context.Players.AddRange(Players);
            context.SaveChanges();

            foreach (var player in Players)
            {
                context.Entry<Player>(player).State = EntityState.Detached;
            }

            return context;
        }

        public static QuakeLoggerContext AddFakeGamePlayers(this QuakeLoggerContext context)
        {
            if (context.GamePlayers.Any()) return context;

            var GamePlayers = new List<GamePlayer>()
            {
                new GamePlayer() {GameId = 1, PlayerId = 1},
                new GamePlayer() {GameId = 1, PlayerId = 2},
                new GamePlayer() {GameId = 1, PlayerId = 3},

                new GamePlayer() {GameId = 2, PlayerId = 1},
                new GamePlayer() {GameId = 2, PlayerId = 2},
                new GamePlayer() {GameId = 2, PlayerId = 3}

            };

            context.GamePlayers.AddRange(GamePlayers);
            context.SaveChanges();

            foreach (var gamePlayer in GamePlayers)
            {
                context.Entry<GamePlayer>(gamePlayer).State = EntityState.Detached;
            }

            return context;
        }
    }
    
}
