using System.Collections.Generic;

namespace QuakeLogger.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int TotalKills { get; set; }
        public List<Player> Players { get; set; }

    }
}
