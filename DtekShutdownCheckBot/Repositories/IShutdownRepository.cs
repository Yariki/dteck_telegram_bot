using System;
using System.Collections.Generic;
using DtekShutdownCheckBot.Shared.Entities;

namespace DtekShutdownCheckBot.Repositories
{
	public interface IShutdownRepository : IRepository<int, Shutdown>
	{
		IEnumerable<Shutdown> GetAllNotSentShutdowns();
		bool IsExistShutdown(string city, DateTime date);
	}
}
