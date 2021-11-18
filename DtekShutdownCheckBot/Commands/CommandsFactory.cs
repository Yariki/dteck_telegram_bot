using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DtekShutdownCheckBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DtekShutdownCheckBot.Commands
{
	public class CommandsFactory : ICommandsFactory
	{
		private IDictionary<string, Type> _commandTypes = new Dictionary<string, Type>()
		{
			{ "city", typeof(RegisterCommand) },
			{ "clear", typeof(ClearCommand) },
			{ "list", typeof(ListCommand) },
            {"check", typeof(CheckCommand)}
		};

		private readonly IServiceProvider _serviceProvider;
		private IServiceFactory _serviceFactory;

		public CommandsFactory(IServiceProvider serviceProvider, IServiceFactory serviceFactory)
		{
			_serviceProvider = serviceProvider;
			_serviceFactory = serviceFactory;
		}


		public ICommand CreateCommand(Update update)
		{
			var info = ParseCommandAndArgument(update);

			if(info.cmd == null)
            {
				return null;
            }

			if (!_commandTypes.ContainsKey(info.cmd))
			{
				return new DoNothingCommand();
			}

			var command = (ICommand)ActivatorUtilities.CreateInstance(_serviceProvider, _commandTypes[info.cmd], _serviceFactory,
				_serviceFactory.Get<ITelegramBotClient>(), info.arg);
			return command;
		}

		private (string cmd,string arg) ParseCommandAndArgument(Update update)
		{
			if (update == null)
				return (null, null);

			var entities = update.Message.Entities;
			if(entities == null)
            {
				return (null, null);
            }
			if (entities.All(e => e.Type != MessageEntityType.BotCommand))
			{
				return (null, null);
			}

			var botCommand = entities.First(e => e.Type == MessageEntityType.BotCommand);
			var index = Array.IndexOf(entities, botCommand);

			var botCommandValue = update.Message.EntityValues.ElementAt(index);

			if (string.IsNullOrEmpty(botCommandValue))
			{
				return (null, null);
			}

			var argument = update.Message.Text.Replace(botCommandValue, string.Empty).Trim();

			return (botCommandValue.Substring(1), argument);
		}

	}
}
