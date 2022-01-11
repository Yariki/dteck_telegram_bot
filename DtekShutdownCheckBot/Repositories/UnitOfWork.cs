using System;
using System.IO;
using DtekShutdownCheckBot.Data;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Shared.Entities;
using LiteDB;
using Microsoft.Extensions.Options;

namespace DtekShutdownCheckBot.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposedValue;
        private DatabaseContext _context;

        public UnitOfWork(IConfiguration configuration)
        {
            _context = new DatabaseContext(configuration.GetSection("ConnectionStrings:Database")?.Value);
            ChatRepository = new ChatRepository(_context);
            ShutdownRepository = new ShutdownRepository(_context);
        }

        public IRepository<int, Chat> ChatRepository { get; }

        public IShutdownRepository ShutdownRepository { get; }

        public void SaveChanges()
        {
            _context?.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UnitOfWork()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
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
