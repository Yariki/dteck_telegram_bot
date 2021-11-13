using System.Threading.Tasks;
using DtekShutdownCheckBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DtekShutdownCheckBot.Commands
{
	public abstract class Command<T> : ICommand
	{
		private readonly IServiceFactory _serviceFactory;
		private readonly ITelegramBotClient _botClient;

		private T _argument;

		public Command(IServiceFactory serviceFactory, ITelegramBotClient  botClient, T argument)
		{
			_serviceFactory = serviceFactory;
			_botClient = botClient;
			_argument = argument;
		}


		protected T Argument => _argument;

		protected ITelegramBotClient BotClient => _botClient;

		protected IServiceFactory ServiceFactory => _serviceFactory;


		public abstract Task ExecuteAsync(Message message);
	}
}
