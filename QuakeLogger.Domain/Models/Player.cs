using QuakeLogger.Domain.Interfaces.Models;
using QuakeLogger.Domain.Models;
using System.Collections.Generic;

namespace QuakeLogger.Models
{
    public class Player : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Kills { get; set; }       
        public IEnumerable<GamePlayer> GamePlayers { get; set; }
    }
}
