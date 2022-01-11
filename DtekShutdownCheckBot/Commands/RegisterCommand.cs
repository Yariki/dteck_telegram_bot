using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Repositories;
using DtekShutdownCheckBot.Services;
using DtekShutdownCheckBot.Shared.Entities;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using Chat = DtekShutdownCheckBot.Shared.Entities.Chat;

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
            using (var unitOfWork = ServiceFactory.Get<IUnitOfWork>())
            {

                var chat = unitOfWork.ChatRepository.GetBy(c => c.ChatId == message.Chat.Id, "Words");
                if (chat == null)
                {
                    chat = new Chat()
                    {
                        ChatId = message.Chat.Id,
                        FirstName = message.Chat.FirstName,
                        LastName = message.Chat.LastName,
                        Description = message.Chat.Description,
                        Title = message.Chat.Title,
                        Bio = message.Chat.Bio,
                        Username = message.Chat.Username
                    };

                    unitOfWork.ChatRepository.Add(chat);
                    unitOfWork.SaveChanges();
                }

                if (chat.Words == null && !string.IsNullOrEmpty(Argument))
                {
                    chat.Words = new List<Word>(){new Word(){Value = Argument}};
                    unitOfWork.ChatRepository.Update(chat);
                }
                else if (!string.IsNullOrEmpty(Argument) && chat.Words.Any())
                {
                    chat.Words.Add(new Word(){Value = Argument});
                    unitOfWork.ChatRepository.Update(chat);
                }
                
                unitOfWork.SaveChanges();

                if (!string.IsNullOrEmpty(Argument))
                {
                    await BotClient.SendTextMessageAsync(chat.ChatId, $"The {Argument} has been registered");

                    ServiceFactory.Get<IMediator>()?.Publish(new CheckForEvent(new List<long>() { chat.ChatId }));
                }
            }
        }
	}
}
