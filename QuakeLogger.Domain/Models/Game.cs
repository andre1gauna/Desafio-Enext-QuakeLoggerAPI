using QuakeLogger.Domain.Interfaces.Models;
using QuakeLogger.Domain.Models;
using System.Collections.Generic;

namespace QuakeLogger.Models
{
    public class Game : IEntity
    {
        public int Id { get; set; }
        public int TotalKills { get; set; }
        public List<KillMethod> KillMethods { get; set; }
        public ICollection<GamePlayer> GamePlayers { get; set; }

        public override bool Equals(object obj)
        {
            return this.Id == ((Game)obj).Id;
        }
        public override int GetHashCode()
        {
            return (this.Id.ToString() + '|' + this.TotalKills.ToString()).GetHashCode();
        }

    }
}
