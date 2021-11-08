using System;
using System.Collections.Generic;
using DtekShutdownCheckBot.Models.Entities;

namespace DtekShutdownCheckBot.Repositories
{
	public interface IShutdownRepository : IRepository<string, Shutdown>
	{
		IEnumerable<Shutdown> GetAllNotSentShutdowns();
		bool IsExistShutdown(string city, DateTime date);
	}
}
