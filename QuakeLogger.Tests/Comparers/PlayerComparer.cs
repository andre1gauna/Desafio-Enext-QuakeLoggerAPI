using QuakeLogger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuakeLogger.Tests.Comparers
{
    public class PlayerComparer : IEqualityComparer<Player>
    {
        public bool Equals(Player x, Player y)
        {
            return x.Id == y.Id && x.PlayerGames == x.PlayerGames;
        }

        public int GetHashCode(Player obj)
        {
            return (obj.Id.ToString() + '|' + obj.Name.ToString()).GetHashCode();
        }
    }
}
