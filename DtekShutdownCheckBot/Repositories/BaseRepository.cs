using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using DtekShutdownCheckBot.Data;
using DtekShutdownCheckBot.Models;
using LiteDB;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;

namespace DtekShutdownCheckBot.Repositories
{
    public abstract class BaseRepository<TKey, TEntity> : IRepository<TKey,TEntity> where TEntity  : class
    {
        private DatabaseContext _context;

        protected BaseRepository(DatabaseContext context)
        {
            _context = context;
        }

        protected DbSet<TEntity> Set => _context.Set<TEntity>();

        public void Add(TEntity model)
        {
            Set.Add(model);
        }

        public abstract TEntity GetById(TKey key, string include = null);

        public TEntity GetBy(Expression<Func<TEntity,bool>> selector,  string include = null)
        {
            if (selector == null)
            {
                return null;
            }

            var result = Set.Where(selector);
            
            if (!string.IsNullOrEmpty(include))
            {
                result = result.Include(include);
            }

            return result.FirstOrDefault();
        }

        public IEnumerable<TEntity> GetAllBy(Expression<Func<TEntity, bool>> selector, string include = null)
        {
            if (selector == null)
            {
                throw new NullReferenceException(nameof(selector));
            }
            var query = !string.IsNullOrEmpty(include) ? Set.Include(include).Where(selector) : Set.Where(selector);

            return query.ToList();
        }

        public IEnumerable<TEntity> GetAll(string include = null)
        {
            IQueryable<TEntity> query = Set;
            if (!string.IsNullOrEmpty(include))
            {
                query = query.Include(include);
            }

            return query.ToList();
        }

        public abstract void Update(TEntity entity);

        public abstract void Delete(TKey key);
        
    }
}
