using System;
using System.Collections.Generic;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Models.Entities;
using DtekShutdownCheckBot.Repositories;
using MediatR;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace DtekShutdownCheckBot.Services
{
    public class DtekCheckingHostedService : IHostedService, IDisposable
    {

        private readonly IServiceFactory _serviceFactory;
        private readonly IMediator _mediator;
	    private Timer _timer;

        public DtekCheckingHostedService(IServiceFactory serviceFactory,
	        IMediator mediator)
        {
            _serviceFactory = serviceFactory;
            _mediator = mediator;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
	        _timer = new Timer(DoCheck, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
	        return Task.CompletedTask;
        }

        private async void DoCheck(object? state)
        {
	        _mediator?.Publish(new CheckForEvent(null));

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
