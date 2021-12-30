using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Shared.Entities;
using DtekShutdownCheckBot.Repositories;
using DtekShutdownCheckBot.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace DtekShutdownCheckBot.Handlers
{
	public class CheckForEvensHandler : INotificationHandler<CheckForEvent>
	{
		const string DTEK_URL = "https://www.dtek-krem.com.ua/ua/outages?query=&rem=%D0%91%D0%BE%D1%80%D0%B8%D1%81%D0%BF%D1%96%D0%BB%D1%8C%D1%81%D1%8C%D0%BA%D0%B8%D0%B9&type=-1&status=-1&shutdown-date=-1&inclusion-date=-1&create-date=-1&page=";

        private const string STREET_LIST_TEMPLATE = "^\\s*{0}:(?<streets>.*)$";

		private const int NUMBER_PAGES = 5;

		private readonly IServiceFactory _serviceFactory;
		private readonly ITelegramBotClient _telegramBotClient;
		private readonly IMediator _mediator;
		private readonly ILogger<CheckForEvensHandler> _logger;

		public CheckForEvensHandler(IServiceFactory serviceFactory, ITelegramBotClient telegramBotClient, IMediator mediator,
			ILogger<CheckForEvensHandler> logger)
		{
			_serviceFactory = serviceFactory;
			_telegramBotClient = telegramBotClient;
			_mediator = mediator;
			_logger = logger;
		}
		
		public async Task Handle(CheckForEvent notification, CancellationToken cancellationToken)
		{
			if (notification == null)
			{
				return;
			}

            var shutdowns = new List<Shutdown>();
			using (var unitOfWork = _serviceFactory.Get<IUnitOfWork>())
			{
				var words = notification.ChatIds != null && notification.ChatIds.Any()
					? unitOfWork.ChatRepository.GetAllBy(c => notification.ChatIds.Contains(c.ChatId))
						.SelectMany(c => c.Words).Distinct()
					: unitOfWork.ChatRepository.GetAll().SelectMany(c => c.Words).Distinct();
				
				if (words == null || !words.Any())
				{
					return;
				}

				try
				{
					
					for (int i = 1; i <= NUMBER_PAGES; i++)
					{
						var url = $"{DTEK_URL}{i}";
						using var client = new HttpClient();

						var content = await client.GetStringAsync(new Uri(url));
						if (string.IsNullOrEmpty(content))
							continue;
						HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
						doc.LoadHtml(content);
						var col = doc.DocumentNode.SelectNodes("//tr[@data-id]");

						foreach (var element in col)
						{
							try
							{
								
								var children = element.ChildNodes
									.Where(c => c.GetType() == typeof(HtmlAgilityPack.HtmlNode))
									.ToList();
								var existingWords = words.Where(w =>
									{
										var pattern = Regex.Escape($"{w}");

										// var srcEnc = Encoding.GetEncoding(1252);
										// var srcBytes = srcEnc.GetBytes(children[3].InnerText);
										// var dstBytes = Encoding.Convert(srcEnc, Encoding.Default, srcBytes);
										// var input = Encoding.Default.GetString(dstBytes).Trim();
										var input = children[3].InnerText;

										var match = input.Contains(pattern,
											StringComparison.InvariantCultureIgnoreCase);
										return match;
									}
								).Select(w =>
                                {
                                    var streetMatch = Regex.Match(children[3].InnerText,
                                        string.Format(STREET_LIST_TEMPLATE, w),
                                        RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                    string street = string.Empty;

                                    if (streetMatch.Success && streetMatch.Groups.Count > 0)
                                    {
                                        street = streetMatch.Groups["streets"].Value;
                                    }

                                    return (city: w, streets: street);
                                }).ToList();

								if (!existingWords.Any())
								{
									continue;
								}
                                


								var date = DateTime.Parse(children[0].InnerText.Trim(),
									new CultureInfo("uk-UA", false));
								var timeOfTheEvent = children[6].InnerText.Trim();

								// create shutdown instance with all info and check if there is by comparing with hashcode.

								foreach (var existingWord in existingWords)
								{
									if (notification.ChatIds == null && unitOfWork.ShutdownRepository.IsExistShutdown(existingWord.city, date))
									{
										continue;
									}

									if (notification.ChatIds == null && shutdowns.Any(s => s.Hashcode == (existingWord.GetHashCode() ^ date.GetHashCode())))
									{
										continue;
									}
									
									shutdowns.Add(new Shutdown(date, existingWord.city,timeOfTheEvent, existingWord.streets));
								}
							}
							catch (Exception ex)
							{
								_logger.LogError(ex.ToString());
							}
						}
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex.ToString());
				}

				if (!shutdowns.Any())
				{
					return;
				}
				
                if (notification.ChatIds != null && notification.ChatIds.Any())
                {
                   
                    await _mediator?.Publish(new NewEvents(notification.ChatIds, shutdowns), cancellationToken);
                }
                else if(shutdowns.Any())
                {
                    foreach (var shutdown in shutdowns)
                    {
                       unitOfWork.ShutdownRepository.Add(shutdown);
                    }
                    await _mediator?.Publish(new NewEvents(), cancellationToken);
                }
            }
		}
	}
}
