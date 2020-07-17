using QuakeLogger.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuakeLogger.Domain.Interfaces.Repositories
{
    public interface IRepositoryBase<T> where T : class, IEntity
    {
        public int Add(T entity);
        public T FindById(int id);

        public void Update(T entity);
        public void Remove(int id);
        
    }
}
