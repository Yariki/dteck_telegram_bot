using DtekShutdownCheckBot.Repositories;
using DtekShutdownCheckBot.Services;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DtekShutdownCheckBot.Commands
{
    public class ListCommand : Command<object>
    {
        public ListCommand(IServiceFactory serviceFactory, ITelegramBotClient botClient, object argument) : base(serviceFactory, botClient, argument)
        {
        }

        public override async Task ExecuteAsync(Message message)
        {
            using var unitOfWork = ServiceFactory.Get<IUnitOfWork>();

            var chat = unitOfWork.ChatRepository.GetBy(c => c.ChatId == message.Chat.Id, "Words");

            if(chat == null)
            {
                return;
            }

            await BotClient.SendTextMessageAsync(chat.ChatId, string.Join(',',chat.Words.Select(w => w.Value)));
        }
    }
}
