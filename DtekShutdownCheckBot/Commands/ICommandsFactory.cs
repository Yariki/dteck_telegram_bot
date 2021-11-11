using Telegram.Bot.Types;

namespace DtekShutdownCheckBot.Commands
{
	public interface ICommandsFactory
	{
		ICommand CreateCommand(Update update);
	}
}
