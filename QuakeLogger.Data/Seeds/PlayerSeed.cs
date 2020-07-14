using QuakeLogger.Data.Context;
using QuakeLogger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuakeLogger.Data.Seeds
{
    public class PlayerSeed
    {
        private readonly QuakeLoggerContext _context;

        public PlayerSeed(QuakeLoggerContext context)
        {
            _context = context;
        }

        public void Populate()
        {           

            Player pl1 = new Player
            {
                Id = 1,
                Name = "pl1",
                Kills = 13,
                GameId = 1
            };

            Player pl2 = new Player
            {
                Id = 2,
                Name = "pl2",
                Kills = 19,
                GameId = 1
            };

            Player pl3 = new Player
            {
                Id = 3,
                Name = "pl3",
                Kills = 8,
                GameId = 1
            };

            _context.Players.AddRange(pl1, pl2, pl3);
            _context.SaveChanges();
;               
        }
    }
}
