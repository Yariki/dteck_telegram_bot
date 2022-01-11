using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DtekShutdownCheckBot.Repositories
{
    public interface IRepository<TKey,TEntity> where TEntity : class
    {
        TEntity GetById(TKey key, string include = null);
        TEntity GetBy(Expression<Func<TEntity,bool>> selector, string include = null);
        void Update(TEntity entity);
        void Delete(TKey key);
        IEnumerable<TEntity> GetAll(string include = null);
        IEnumerable<TEntity> GetAllBy(Expression<Func<TEntity, bool>> selector, string include = null);
        void Add(TEntity model);
    }
}