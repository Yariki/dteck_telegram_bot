using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using DtekShutdownCheckBot.Models;
using LiteDB;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;

namespace DtekShutdownCheckBot.Repositories
{
    public abstract class BaseRepository<TKey, TEntity> : IRepository<TKey,TEntity> where TEntity  : class
    {
        private LiteDatabase _db;
        private IOptions<LiteDbOptions> _options;

        protected BaseRepository(IOptions<LiteDbOptions> options)
        {
            _options = options;
            _db = new LiteDatabase(GetConnectionString(_options.Value));
            Set = _db.GetCollection<TEntity>() as LiteCollection<TEntity>;
        }

        protected LiteCollection<TEntity> Set { get; }

        public void Add(TEntity model)
        {
	        Set.Insert(model);
        }

        public abstract TEntity GetById(TKey key);

        public TEntity GetBy(Expression<Func<TEntity,bool>> selector)
        {
            if (selector == null)
            {
                return null;
            }
            return Set.Find(selector).FirstOrDefault();
        }

        public IEnumerable<TEntity> GetAllBy(Expression<Func<TEntity, bool>> selector) => Set.Find(selector);

        public IEnumerable<TEntity> GetAll() => Set.FindAll();

        public void Update(TEntity entity)
        {
            Set.Update(entity);
        }

        public abstract void Delete(TKey key);

        public void DeleteAll() => Set.DeleteAll();

        public void Dispose()
        {
            _db.Dispose();
        }

        private string GetConnectionString(LiteDbOptions options)
        {
	        var di = new DirectoryInfo(options.FolderName);
	        if (!di.Exists)
	        {
		        di.Create();
	        }

	        return Path.Combine(options.FolderName, options.FileName);
        }
    }
}
