using QuakeLogger.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuakeLogger.Tests.Comparers
{
    public class GamePlayerComparer
    {
        public bool Equals(GamePlayer x, GamePlayer y)
        {
            return x.GameId == y.GameId
                && x.PlayerId == y.PlayerId;
        }

        public int GetHashCode(GamePlayer obj)
        {
            return (obj.GameId.ToString() + '|' + obj.PlayerId.ToString()).GetHashCode();
        }
    }
}
