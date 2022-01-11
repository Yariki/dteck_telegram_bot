using System;
using System.Collections.Generic;
using System.Linq;
using DtekShutdownCheckBot.Data;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Shared.Entities;
using LiteDB;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Options;

namespace DtekShutdownCheckBot.Repositories
{
    public class ShutdownRepository : BaseRepository<int, Shutdown>, IShutdownRepository
    {
        public ShutdownRepository(DatabaseContext context) : base(context)
        {
        }

        public override Shutdown GetById(int key, string include = null)
        {
            return GetBy(s => s.Id == key, include);
        }

        public override void Delete(int key)
        {
            var item = GetById(key);
            if (item == null)
            {
                throw new NullReferenceException(nameof(item));
            }

            Set.Remove(item);
        }

        public override void Update(Shutdown entity)
        {
            var item = GetById(entity.Id);
            if (item == null)
            {
                throw new NullReferenceException(nameof(item));
            }

            item = entity;
            Set.Update(item);
        }

        public IEnumerable<Shutdown> GetAllNotSentShutdowns()
        {
            return GetAllBy(s => !s.IsSent);
        }

        public bool IsExistShutdown(string city, DateTime date)
        {
	        return GetAll().Any(s => string.Equals(s.City, city, StringComparison.InvariantCultureIgnoreCase) &&
	                                 s.ShutdownDate == date);
        }

    }
}
