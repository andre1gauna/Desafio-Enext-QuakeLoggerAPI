using QuakeLogger.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuakeLogger.Domain.Interfaces.Repositories
{
    public interface IQuakePlayerRepo : IRepositoryBase<Player>
    {
        public Player FindByName(string name);
        public List<Player> GetAll();
        public void Update(Player player);
    }
}
