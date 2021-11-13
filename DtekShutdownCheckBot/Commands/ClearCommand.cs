using DtekShutdownCheckBot.Repositories;
using DtekShutdownCheckBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DtekShutdownCheckBot.Commands
{
    public class ClearCommand : Command<object>
    {
        public ClearCommand(IServiceFactory serviceFactory, ITelegramBotClient botClient, object argument) : base(serviceFactory, botClient, argument)
        {
        }

        public override async Task ExecuteAsync(Message message)
        {
            using var unitOfWork = ServiceFactory.Get<IUnitOfWork>();
            var chat = unitOfWork.ChatRepository.GetBy(c => c.ChatId == message.Chat.Id);

            if(chat == null)
            {
                return;
            }

            chat.Words = new string[] { };
            unitOfWork.ChatRepository.Update(chat);

            await BotClient.SendTextMessageAsync(chat.ChatId, "The list of cities was cleared");
        }
    }
}
