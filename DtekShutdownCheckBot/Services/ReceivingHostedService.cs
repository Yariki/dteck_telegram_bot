using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly IServiceCollection _serviceCollection;

        public ReceivingHostedService(ITelegramBotClient client, IServiceCollection serviceCollection)
        {
            _client = client;
            _serviceCollection = serviceCollection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _client.StartReceiving(new TelegramDtekUpdateHandler(), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
    }
}