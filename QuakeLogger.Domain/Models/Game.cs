using QuakeLogger.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;

namespace QuakeLogger.Models
{
    public class Game : IEntity
    {
        public int Id { get; set; }
        public int TotalKills { get; set; }
        public IEnumerable<Player> Players { get; set; }

        
    }
}
