using QuakeLogger.Domain.Interfaces.Models;
using QuakeLogger.Domain.Models;
using System.Collections.Generic;

namespace QuakeLogger.Models
{
    public class Player : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }               
        public ICollection<GamePlayer> PlayerGames { get; set; }

        public override bool Equals(object obj)
        {
            return this.Name == ((Player)obj).Name;
        }

        public override int GetHashCode()
        {
            return (this.Id.ToString() + '|' + this.Name.ToString()).GetHashCode();
        }
    }
}
