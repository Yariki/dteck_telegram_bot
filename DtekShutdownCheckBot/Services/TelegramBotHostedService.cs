using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Commands;
using DtekShutdownCheckBot.Models.Entities;
using DtekShutdownCheckBot.Repositories;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace DtekShutdownCheckBot.Services
{
    public class TelegramBotHostedService : BackgroundService
    {
        private ITelegramBotClient _client;
        private readonly ICommandsFactory _commandsFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<TelegramBotHostedService> _logger;


        public TelegramBotHostedService(ITelegramBotClient client, ICommandsFactory commandsFactory, ILoggerFactory loggerFactory)
        {
	        _client = client;
	        _commandsFactory = commandsFactory;
	        _loggerFactory = loggerFactory;
	        _logger = _loggerFactory.CreateLogger<TelegramBotHostedService>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
	        try
	        {
		        _client.StartReceiving(new TelegramBotUpdateHandler(_commandsFactory, _loggerFactory.CreateLogger<TelegramBotUpdateHandler>()), stoppingToken);

		        while (!stoppingToken.IsCancellationRequested)
		        {
			        await Task.Delay(1000);
		        }
	        }
	        catch (Exception e)
	        {
		        Console.WriteLine(e);
		        _logger.LogError(e.ToString());
	        }
        }
    }
}
