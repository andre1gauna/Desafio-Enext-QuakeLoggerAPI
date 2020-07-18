using Microsoft.EntityFrameworkCore;
using QuakeLogger.Data.Context;
using QuakeLogger.Domain.Interfaces.Repositories;
using QuakeLogger.Domain.Models;
using QuakeLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuakeLogger.Data.Repositories
{

    public class QuakePlayerRepo : IQuakePlayerRepo
    {
        private readonly QuakeLoggerContext _context;

        public QuakePlayerRepo(QuakeLoggerContext context)
        {
            _context = context;
        }
        
        public int Add(Player player)
        {
            _context.Players.Add(player);
            _context.SaveChanges();

            return player.Id;
        }

        public List<Player> GetAll()
        {
            return _context.Players.Include(x => x.PlayerGames).ToList();
        }
        public Player FindById(int id)
        {
            return _context.Players
                .Where(i => i.Id == id).Include(x => x.PlayerGames)
                .FirstOrDefault();
        }        

        public Player FindByName(string name)
        {
            return _context.Players
                .Where(i => i.Name == name).Include(x => x.PlayerGames)
                .FirstOrDefault();
        }
        public IQueryable<GamePlayer> FindByGameId(int gameId)
        {
            return _context
                .GamePlayers
                .Where(i => i.GameId == gameId);
        }
        public void Update(Player player)
        {
            _context.Players.Update(player);
            _context.SaveChanges();
        }

        
    }
}
