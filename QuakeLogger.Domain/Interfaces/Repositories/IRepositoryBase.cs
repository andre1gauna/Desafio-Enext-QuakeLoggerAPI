using QuakeLogger.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuakeLogger.Domain.Interfaces.Repositories
{
    public interface IRepositoryBase<T> where T : class, IEntity
    {
        public T FindById(int id);        

        public void Remove(int id);
        
    }
}
