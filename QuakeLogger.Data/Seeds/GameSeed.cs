using QuakeLogger.Data.Context;
using QuakeLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuakeLogger.Data.Seeds
{
    public class GameSeed
    {
        private readonly QuakeLoggerContext _context;

        public GameSeed(QuakeLoggerContext context)
        {
            _context = context;
        }

        public void Populate()
        {
            Game game = new Game
            {
                Id = 1,
                TotalKills = 40, // soma hard-coded das kills dos players
                Players = _context.Players.ToList()
            };

            _context.Games.Add(game);
            _context.SaveChanges();
        }
    }
}
