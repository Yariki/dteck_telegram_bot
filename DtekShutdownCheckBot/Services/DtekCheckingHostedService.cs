﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Models.Entities;
using DtekShutdownCheckBot.Repositories;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic;

namespace DtekShutdownCheckBot.Services
{
    public class DtekCheckingHostedService : IHostedService, IDisposable
    {
	    const string DTEK_URL = "https://www.dtek-krem.com.ua/ua/outages?query=&rem=%D0%91%D0%BE%D1%80%D0%B8%D1%81%D0%BF%D1%96%D0%BB%D1%8C%D1%81%D1%8C%D0%BA%D0%B8%D0%B9&type=-1&status=-1&shutdown-date=-1&inclusion-date=-1&create-date=-1&page=";

	    private const int NUMBER_PAGES = 3;

	    private readonly IRepository<string, Chat> _charRepository;
	    private readonly IShutdownRepository _shutdownRepository;
	    private readonly IMediator _mediator;
	    private Timer _timer;

        public DtekCheckingHostedService(IRepository<string, Chat> charRepository,
	        IShutdownRepository shutdownRepository,
	        IMediator mediator)
        {
	        _charRepository = charRepository;
	        _shutdownRepository = shutdownRepository;
	        _mediator = mediator;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
	        _timer = new Timer(DoCheck, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
	        return Task.CompletedTask;
        }

        private async void DoCheck(object? state)
        {
	        var words = _charRepository.GetAll().SelectMany(c => c.Words).Distinct().ToList();

			if(words == null || !words.Any())
            {
				return;
            }

	        var shutdowns = new Dictionary<string, HashSet<DateTime>>();

	        for (int i = 1; i <= NUMBER_PAGES; i++)
	        {
		        var url = $"{DTEK_URL}{i}";
		        using var client = new HttpClient();
		        var content = await client.GetStringAsync(new Uri(url));
		        if(string.IsNullOrEmpty(content))
			        continue;
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(content);
                var col = doc.DocumentNode.SelectNodes("//tr[@data-id]");

                foreach (var element in col)
                {
                    var children = element.ChildNodes.Where(c => c.GetType() == typeof(HtmlAgilityPack.HtmlNode)).ToList();
                    var existingWords = words.Where(w =>
	                    {
		                    var subString = Regex.Escape(w);
		                    var input = children[3].InnerText.Replace("\n", string.Empty).Trim();
		                    var match = Regex.IsMatch(input, subString, RegexOptions.IgnoreCase | RegexOptions.Multiline);
		                    return match;
	                    }
	                    ).Select(w => w).ToList();
                    var date = DateTime.Parse(children[0].InnerText.Trim());

                    foreach (var existingWord in existingWords)
                    {
	                    if (_shutdownRepository.IsExistShutdown(existingWord, date))
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
		                    shutdowns.Add(existingWord, new HashSet<DateTime>(){date});
	                    }
                    }
                }
	        }

	        if (!shutdowns.Any())
	        {
		        return;
	        }


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
			        _shutdownRepository.Add(model);
		        }
	        }

	        if (shutdowns.Any())
	        {
		        _mediator?.Publish(new NewEvents());
	        }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
	        _timer?.Change(Timeout.Infinite, 0);

	        return Task.CompletedTask;
        }

        public void Dispose()
        {
	        _timer?.Dispose();
        }
    }
}
