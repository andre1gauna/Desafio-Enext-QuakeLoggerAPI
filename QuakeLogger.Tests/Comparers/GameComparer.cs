using QuakeLogger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuakeLogger.Tests.Comparers
{
    public class GameComparer : IEqualityComparer<Game>
    {
        public bool Equals (Game x, Game y)
        {
            return x.Id == y.Id && x.GamePlayers == x.GamePlayers;
        }

        public int GetHashCode(Game obj)
        {
            return (obj.Id.ToString() + '|' + obj.TotalKills.ToString()).GetHashCode();
        }
    }
}
