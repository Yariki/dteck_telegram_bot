using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Models.Entities;
using DtekShutdownCheckBot.Repositories;
using MediatR;
using Telegram.Bot;

namespace DtekShutdownCheckBot.Handlers
{
	public class NewEventsHandler : INotificationHandler<NewEvents>
	{
		private readonly IRepository<string, Chat> _chatRepository;
		private readonly IRepository<string, Shutdown> _shutdownRepository;
		private readonly ITelegramBotClient _telegramBotClient;

		public NewEventsHandler(IRepository<string, Chat> chatRepository,
			IRepository<string, Shutdown> shutdownRepository,
			ITelegramBotClient telegramBotClient)
		{
			_chatRepository = chatRepository;
			_shutdownRepository = shutdownRepository;
			_telegramBotClient = telegramBotClient;
		}

		public Task Handle(NewEvents notification, CancellationToken cancellationToken)
		{
			var newEvents = _shutdownRepository.GetAllBy(s => !s.IsSent);
			if (!newEvents.Any())
			{
				return Task.CompletedTask;
			}

			foreach (var newEvent in newEvents)
			{
				var chats = _chatRepository.GetAllBy(c => c.Words.Contains(newEvent.City));
				if (!chats.Any())
				{
					continue;
				}

				foreach (var chat in chats)
				{
					_telegramBotClient.SendTextMessageAsync(chat.ChatId,
						$"Planned shutdown in {newEvent.City} on {newEvent.ShutdownDate}");
				}

				newEvent.IsSent = true;
				_shutdownRepository.Update(newEvent);
			}

			return Task.CompletedTask;
		}
	}
}
