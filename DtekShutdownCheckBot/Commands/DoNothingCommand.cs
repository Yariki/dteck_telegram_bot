using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DtekShutdownCheckBot.Commands
{
	public class DoNothingCommand : ICommand
	{
		public Task ExecuteAsync(Message message)
		{
			return Task.CompletedTask;
		}
	}
}
