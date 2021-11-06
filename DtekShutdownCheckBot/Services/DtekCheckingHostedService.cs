using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Models.Entities;
using DtekShutdownCheckBot.Repositories;
using Microsoft.Extensions.Hosting;

namespace DtekShutdownCheckBot.Services
{
    public class DtekCheckingHostedService : IHostedService, IDisposable
    {
	    const string DTEK_URL = "https://www.dtek-krem.com.ua/ua/outages?query=&rem=%D0%91%D0%BE%D1%80%D0%B8%D1%81%D0%BF%D1%96%D0%BB%D1%8C%D1%81%D1%8C%D0%BA%D0%B8%D0%B9&type=-1&status=-1&shutdown-date=-1&inclusion-date=-1&create-date=-1&page=";

	    private const int NUMBER_PAGES = 3;

	    private readonly IRepository<string, Chat> _charRepository;
	    private Timer _timer;

        public DtekCheckingHostedService(IRepository<string, Chat> charRepository)
        {
	        _charRepository = charRepository;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
	        _timer = new Timer(DoCheck, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
	        return Task.CompletedTask;
        }

        private async void DoCheck(object? state)
        {
	        var words = _charRepository.GetAll().SelectMany(c => c.Words).Distinct();

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
                var dict = new Dictionary<string, DateTime>();

                foreach (var element in col)
                {   
                    var childs = element.ChildNodes.Where(c => c.GetType() == typeof(HtmlAgilityPack.HtmlNode)).ToList();
                    var existingWords = words.Where(w =>
                        childs[3].InnerText.Contains(w, StringComparison.InvariantCultureIgnoreCase));
                    
                    foreach (var existingWord in existingWords)
                    {
                        if(dict.ContainsKey(existingWord))
                            continue;
                        dict.Add(existingWord, DateTime.Parse(childs[0].InnerText.Trim()));
                    }

                    
                }
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
