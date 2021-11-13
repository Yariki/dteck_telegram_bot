using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Models.Entities;
using DtekShutdownCheckBot.Repositories;
using DtekShutdownCheckBot.Services;
using MediatR;
using Telegram.Bot;

namespace DtekShutdownCheckBot.Handlers
{
	public class NewEventsHandler : INotificationHandler<NewEvents>
	{
        private readonly IServiceFactory _serviceFactory;
        private readonly ITelegramBotClient _telegramBotClient;

		public NewEventsHandler(IServiceFactory serviceFactory,
			ITelegramBotClient telegramBotClient)
		{
            _serviceFactory = serviceFactory;
            _telegramBotClient = telegramBotClient;
		}

		public Task Handle(NewEvents notification, CancellationToken cancellationToken)
		{
			using var unitOfWork = _serviceFactory.Get<IUnitOfWork>();
			var newEvents = unitOfWork.ShutdownRepository.GetAllNotSentShutdowns();
			if (!newEvents.Any())
			{
				return Task.CompletedTask;
			}

			foreach (var newEvent in newEvents.OrderBy(e => e.ShutdownDate))
			{
				var chats = unitOfWork.ChatRepository.GetAllBy(c => c.Words.Contains(newEvent.City));
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
				unitOfWork.ShutdownRepository.Update(newEvent);

			}
			return Task.CompletedTask;
		}
	}
}
