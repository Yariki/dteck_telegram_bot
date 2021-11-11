using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace DtekShutdownCheckBot.Commands
{
	public interface ICommand
	{
		Task ExecuteAsync(Message message);
	}
}
