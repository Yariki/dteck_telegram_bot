using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DtekShutdownCheckBot.Services
{
    public class TelegramDtekUpdateHandler : IUpdateHandler
    {




        public Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

	        return Task.CompletedTask;
        }

        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
	        return Task.CompletedTask;
        }

        public UpdateType[]? AllowedUpdates { get; }
    }
}
