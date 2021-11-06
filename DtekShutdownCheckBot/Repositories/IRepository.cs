using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DtekShutdownCheckBot.Repositories
{
    public interface IRepository<TKey,TEntity> : IDisposable where TEntity : class
    {
        TEntity GetById(TKey key);
        TEntity GetBy(Expression<Func<TEntity,bool>> selector);
        void Update(TEntity entity);
        void Delete(TKey key);
        IEnumerable<TEntity> GetAll();
        void DeleteAll();
        IEnumerable<TEntity> GetAllBy(Expression<Func<TEntity, bool>> selector);
        void Add(TEntity model);
    }
}