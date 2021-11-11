using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Commands;
using DtekShutdownCheckBot.Models.Entities;
using DtekShutdownCheckBot.Repositories;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace DtekShutdownCheckBot.Services
{
    public class ReceivingHostedService : BackgroundService
    {
        private ITelegramBotClient _client;
        private readonly IRepository<string, Chat> _chatRepository;
        private readonly ICommandsFactory _commandsFactory;

        public ReceivingHostedService(ITelegramBotClient client,
	        IRepository<string,Chat> chatRepository,
	        ICommandsFactory commandsFactory)
        {
	        _client = client;
	        _chatRepository = chatRepository;
	        _commandsFactory = commandsFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _client.StartReceiving(new TelegramDtekUpdateHandler(_chatRepository, _commandsFactory), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
    }
}
