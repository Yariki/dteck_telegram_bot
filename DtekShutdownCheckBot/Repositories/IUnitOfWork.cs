using DtekShutdownCheckBot.Models.Entities;

namespace DtekShutdownCheckBot.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<string, Chat> ChatRepository { get; }
        IShutdownRepository ShutdownRepository { get; }
    }
}
