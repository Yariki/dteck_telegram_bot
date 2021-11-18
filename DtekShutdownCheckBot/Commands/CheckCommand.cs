using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Services;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DtekShutdownCheckBot.Commands;

public class CheckCommand : Command<object>
{
    public CheckCommand(IServiceFactory serviceFactory, ITelegramBotClient botClient, object argument) : base(serviceFactory, botClient, argument)
    {
    }

    public override Task ExecuteAsync(Message message)
    {
        var currentChatId = message.Chat.Id;
        var mediator = ServiceFactory.Get<IMediator>();
        mediator.Publish(new CheckForEvent(new List<long>() { currentChatId }));

        return Task.CompletedTask;
    }
}