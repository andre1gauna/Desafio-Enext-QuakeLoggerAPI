using QuakeLogger.Domain.Enums;
using QuakeLogger.Domain.Interfaces.Models;
using QuakeLogger.Domain.Models;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuakeLogger.Models
{
    public class Game : IEntity
    {
        public int Id { get; set; }
        public int TotalKills { get; set; }        
        public ICollection<GamePlayer> GamePlayers { get; set; }

    }
}
