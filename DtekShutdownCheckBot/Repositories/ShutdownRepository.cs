using System;
using System.Collections.Generic;
using System.Linq;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Models.Entities;
using Microsoft.Extensions.Options;

namespace DtekShutdownCheckBot.Repositories
{
    public class ShutdownRepository : BaseRepository<string, Shutdown>, IShutdownRepository
    {
        public ShutdownRepository(IOptions<LiteDbOptions> options) : base(options)
        {
        }

        public override Shutdown GetById(string key)
        {
            return GetBy(s => s.Id == key);
        }

        public override void Delete(string key)
        {
            Set.Delete(key);
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
