using System;
using System.Threading;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Models.Entities;
using DtekShutdownCheckBot.Repositories;
using Microsoft.Extensions.Hosting;

namespace DtekShutdownCheckBot.Services
{
    public class DtekCheckingHostedService : IHostedService, IDisposable
    {
	    private readonly IRepository<string, Chat> _charRepository;
	    private Timer _timer;

        public DtekCheckingHostedService(IRepository<string, Chat> charRepository)
        {
	        _charRepository = charRepository;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
	        _timer = new Timer(DoCheck, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
	        return Task.CompletedTask;
        }

        private void DoCheck(object? state)
        {
	        throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
	        _timer?.Change(Timeout.Infinite, 0);

	        return Task.CompletedTask;
        }

        public void Dispose()
        {
	        _timer?.Dispose();
        }
    }
}
