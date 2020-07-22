using QuakeLogger.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuakeLogger.Domain.Interfaces.Repositories
{
    public interface IQuakeKillMethodRepo : IRepositoryBase<KillMethod>
    {
        public KillMethod FindByName(string nameId);
    }
}
