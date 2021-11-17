using System;
using System.IO;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Models.Entities;
using LiteDB;
using Microsoft.Extensions.Options;

namespace DtekShutdownCheckBot.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposedValue;
        private LiteDatabase _db;
        private IOptions<LiteDbOptions> _options;

        public UnitOfWork(IOptions<LiteDbOptions> options)
        {
            _options = options;
            var connectionString = @$"Filename={GetConnectionString(_options.Value)}; Connection=Shared;";
            _db = new LiteDatabase(connectionString);
            ChatRepository = new ChatRepository(_db);
            ShutdownRepository = new ShutdownRepository(_db);

        }

        public IRepository<string, Chat> ChatRepository { get; }

        public IShutdownRepository ShutdownRepository { get; }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _db?.Dispose();
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
