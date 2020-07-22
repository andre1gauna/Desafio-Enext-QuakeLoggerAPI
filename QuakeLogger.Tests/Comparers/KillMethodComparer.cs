using QuakeLogger.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuakeLogger.Tests.Comparers
{
     public class KillMethodComparer
    {
        public bool Equals(KillMethod x, KillMethod y)
        {
            return x.Id == y.Id
                && x.NameId == y.NameId;
        }

        public int GetHashCode(KillMethod obj)
        {
            return (obj.Id.ToString() + '|' + obj.NameId.ToString()).GetHashCode();
        }
    }
}
