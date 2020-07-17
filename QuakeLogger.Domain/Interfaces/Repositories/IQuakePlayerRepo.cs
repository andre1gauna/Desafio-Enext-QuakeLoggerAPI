using QuakeLogger.Domain.Models;
using QuakeLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuakeLogger.Domain.Interfaces.Repositories
{
    public interface IQuakePlayerRepo : IRepositoryBase<Player>
    {
        public Player FindByName(string name);

        public IQueryable<GamePlayer> FindByGameId(int gameId);
        public List<Player> GetAll();
    }
}
