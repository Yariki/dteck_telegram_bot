using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Shared.Entities;
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

		public async Task Handle(NewEvents notification, CancellationToken cancellationToken)
		{
			if (notification.ChatIds != null && notification.Shutdowns != null)
			{
				await SendMessageToParticularChat(notification);
				return;
			}

			await SendMessagesToAllChats();

		}

		private async Task SendMessageToParticularChat(NewEvents newEvents)
		{
			using var unitOfWork = _serviceFactory.Get<IUnitOfWork>();
			var chats = unitOfWork.ChatRepository.GetAllBy(c => newEvents.ChatIds.Contains(c.ChatId)).ToList();

			var stringComparer = new StringComparer();

			foreach (var shutdown in newEvents.Shutdowns.OrderBy(s => s.ShutdownDate))
			{
				var chatsToSend = chats.Where(c => c.Words.Contains(shutdown.City, stringComparer));

				foreach (var chatToSend in chatsToSend)
				{
					await _telegramBotClient.SendTextMessageAsync(chatToSend.ChatId,
						$"Planned shutdown in {shutdown.City} on {shutdown.ShutdownDate.ToShortDateString()}\nTime of event: {shutdown.TimeOfTheEvent}\nStreets: {shutdown.Streets}");
				}
			}
		}

		private async Task SendMessagesToAllChats()
		{
			using var unitOfWork = _serviceFactory.Get<IUnitOfWork>();
			var newEvents = unitOfWork.ShutdownRepository.GetAllNotSentShutdowns().OrderBy(s => s.ShutdownDate).ToList();
			if (!newEvents.Any())
			{
				return;
			}

			foreach (var newEvent in newEvents)
			{
				var chats = unitOfWork.ChatRepository.GetAllBy(c => c.Words.Contains(newEvent.City));
				if (!chats.Any())
				{
					continue;
				}

				foreach (var chat in chats)
				{
					await _telegramBotClient.SendTextMessageAsync(chat.ChatId,
						$"Planned shutdown in {newEvent.City} on {newEvent.ShutdownDate.ToShortDateString()}");
				}

				newEvent.IsSent = true;
				unitOfWork.ShutdownRepository.Update(newEvent);

			}


		}

		class StringComparer : IEqualityComparer<string>
		{
			public bool Equals(string? x, string? y)
			{
				return x.Equals(y, StringComparison.InvariantCultureIgnoreCase);
			}

			public int GetHashCode(string obj)
			{
				return obj.GetHashCode();
			}
		}

	}
}
