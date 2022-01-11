using DtekShutdownCheckBot.Shared.Entities;
using System;

namespace DtekShutdownCheckBot.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<int, Chat> ChatRepository { get; }
        IShutdownRepository ShutdownRepository { get; }

        void SaveChanges();
    }
}
