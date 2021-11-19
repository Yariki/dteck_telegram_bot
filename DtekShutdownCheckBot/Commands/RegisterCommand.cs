using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Repositories;
using DtekShutdownCheckBot.Services;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Chat = DtekShutdownCheckBot.Models.Entities.Chat;

namespace DtekShutdownCheckBot.Commands
{
	public class RegisterCommand : Command<string>
	{

		public RegisterCommand(IServiceFactory serviceFactory,
			ITelegramBotClient botClient,
			string argument) : base(serviceFactory, botClient, argument)
		{

		}

		public override async Task ExecuteAsync(Message message)
		{
			var unitOfWork = ServiceFactory.Get<IUnitOfWork>();

			var chat = unitOfWork.ChatRepository.GetBy(c => c.ChatId == message.Chat.Id);
			if (chat == null)
			{
				chat = new Chat()
				{
					Id = Guid.NewGuid().ToString(),
					ChatId = message.Chat.Id,
					FirstName = message.Chat.FirstName,
					LastName = message.Chat.LastName
				};
				unitOfWork.ChatRepository.Add(chat);
			}

			if(chat.Words == null && !string.IsNullOrEmpty(Argument))
			{
				chat.Words = new List<string>() { Argument }.ToArray();
			}
			else if(!string.IsNullOrEmpty(Argument))
			{
				chat.Words = new List<string>(chat.Words) { Argument }.ToArray();
			}
			unitOfWork.ChatRepository.Update(chat);

			if (!string.IsNullOrEmpty(Argument))
			{
				await BotClient.SendTextMessageAsync(chat.ChatId, $"The {Argument} has been registered");

				ServiceFactory.Get<IMediator>()?.Publish(new CheckForEvent(new List<long>() { chat.ChatId }));
			}

		}
	}
}
