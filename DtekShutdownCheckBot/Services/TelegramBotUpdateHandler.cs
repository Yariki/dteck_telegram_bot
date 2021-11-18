using System;
using System.Threading;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Commands;
using DtekShutdownCheckBot.Repositories;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Chat = DtekShutdownCheckBot.Models.Entities.Chat;

namespace DtekShutdownCheckBot.Services
{
    public class TelegramBotUpdateHandler : IUpdateHandler
    {
	    private readonly ICommandsFactory _commandsFactory;
	    private readonly ILogger<TelegramBotUpdateHandler> _logger;


	    public TelegramBotUpdateHandler(ICommandsFactory commandsFactory, ILogger<TelegramBotUpdateHandler> logger)
	    {
		    _commandsFactory = commandsFactory;
		    _logger = logger;
	    }

        public Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
	        var command = _commandsFactory.CreateCommand(update);

	        command?.ExecuteAsync(update.Message);

	        _logger.LogInformation($"Command {update.Message.Text} for the chat - {update.Message.Chat.Id}");

	        return Task.CompletedTask;
        }

        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
	        return Task.CompletedTask;
        }

        public UpdateType[]? AllowedUpdates { get; }
    }
}
