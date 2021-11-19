using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Models.Entities;
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

		private const int NUMBER_PAGES = 3;

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
			var shutdowns = new Dictionary<string, HashSet<DateTime>>();
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
								).Select(w => w).ToList();

								if (!existingWords.Any())
								{
									continue;
								}

								var date = DateTime.Parse(children[0].InnerText.Trim(),
									new CultureInfo("uk-UA", false));

								foreach (var existingWord in existingWords)
								{
									if (unitOfWork.ShutdownRepository.IsExistShutdown(existingWord, date))
									{
										continue;
									}

									if (shutdowns.ContainsKey(existingWord) && shutdowns[existingWord].Contains(date))
									{
										continue;
									}

									if (shutdowns.ContainsKey(existingWord) && !shutdowns[existingWord].Contains(date))
									{
										shutdowns[existingWord].Add(date);
									}
									else
									{
										shutdowns.Add(existingWord, new HashSet<DateTime>() { date });
									}
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
                    var listOfShutdowns = new List<Shutdown>();

                    foreach (var item in shutdowns)
                    {
	                    listOfShutdowns.AddRange(item.Value.Select(d => new Shutdown()
	                    {
		                    Id = Guid.NewGuid().ToString(),
		                    City = item.Key,
		                    ShutdownDate = d,
		                    Hashcode = item.Key.GetHashCode() ^ d.GetHashCode(),
		                    IsSent = false
	                    }).ToList());
                    }

                    _mediator.Publish(new NewEvents(notification.ChatIds, listOfShutdowns));
                }
                else if(shutdowns.Any())
                {
                    foreach (var shutdown in shutdowns)
                    {
                        foreach (var dateTime in shutdown.Value)
                        {
                            var model = new Shutdown()
                            {
                                Id = Guid.NewGuid().ToString(),
                                City = shutdown.Key,
                                ShutdownDate = dateTime,
                                Hashcode = shutdown.Key.GetHashCode() ^ dateTime.GetHashCode(),
                                IsSent = false
                            };
                            unitOfWork.ShutdownRepository.Add(model);
                        }
                    }

                    _mediator?.Publish(new NewEvents());
                }
            }
		}
	}
}
