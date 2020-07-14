using QuakeLogger.Data.Context;
using QuakeLogger.Domain.Interfaces.Repositories;
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

        public Player FindById(int id)
        {
            return _context.Players
                .Where(i => i.Id == id)
                .FirstOrDefault();
        }       

        public void Remove(int id)
        {
            var player = _context.Players.First(i => i.Id == id);
            _context.Players.Remove(player);
            _context.SaveChanges();
        }
    }
}
