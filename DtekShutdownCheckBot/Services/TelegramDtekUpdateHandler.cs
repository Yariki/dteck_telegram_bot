using System;
using System.Threading;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Commands;
using DtekShutdownCheckBot.Repositories;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Chat = DtekShutdownCheckBot.Models.Entities.Chat;

namespace DtekShutdownCheckBot.Services
{
    public class TelegramDtekUpdateHandler : IUpdateHandler
    {
	    private readonly ICommandsFactory _commandsFactory;

	    public TelegramDtekUpdateHandler(ICommandsFactory commandsFactory)
	    {
		    _commandsFactory = commandsFactory;
	    }

        public Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
	        var command = _commandsFactory.CreateCommand(update);

	        command?.ExecuteAsync(update.Message);

	        return Task.CompletedTask;
        }

        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
	        return Task.CompletedTask;
        }

        public UpdateType[]? AllowedUpdates { get; }
    }
}
