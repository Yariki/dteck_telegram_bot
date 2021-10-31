using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace DtekShutdownCheckBot.Services
{
    public class ReceivingHostedService : IHostedService
    {
        private ITelegramBotClient _client;
        private readonly IServiceCollection _serviceCollection;

        public ReceivingHostedService(ITelegramBotClient client, IServiceCollection serviceCollection)
        {
            _client = client;
            _serviceCollection = serviceCollection;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client.StartReceiving(new TelegramDtekUpdateHandler(), cancellationToken);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            
            return Task.CompletedTask;
        }
    }
}