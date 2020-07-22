using QuakeLogger.Data.Context;
using QuakeLogger.Domain.Interfaces.Repositories;
using QuakeLogger.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuakeLogger.Data.Repositories
{
    public class QuakeKillMethodRepo : IQuakeKillMethodRepo
    {
        private readonly QuakeLoggerContext _context;

        public QuakeKillMethodRepo(QuakeLoggerContext context)
        {
            _context = context;
        }

        public int Add(KillMethod KillMethod)
        {
            _context.KillMethods.Add(KillMethod);
            _context.SaveChanges();

            return KillMethod.Id;
        }

        public KillMethod FindById(int id)
        {
           return  _context.KillMethods
                .Where(i => i.Id == id)
                .FirstOrDefault();          
        }
        public KillMethod FindByName(string nameId)
        {
            return _context.KillMethods
                 .Where(i => i.NameId == nameId)
                 .FirstOrDefault();
        }

        public List<KillMethod> GetAll()
        {
            return _context.KillMethods.ToList();
        }

        public void Update(KillMethod KillMethod)
        {
            _context.KillMethods.Update(KillMethod);
            _context.SaveChanges();
        }
    }
}
