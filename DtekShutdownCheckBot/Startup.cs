using System.Text;
using DtekShutdownCheckBot.Commands;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Models.Entities;
using DtekShutdownCheckBot.Repositories;
using DtekShutdownCheckBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using MediatR;
using ServiceFactory = DtekShutdownCheckBot.Services.ServiceFactory;

namespace DtekShutdownCheckBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            BotConfig = Configuration.GetSection("BotConfiguration").Get<BotConfiguration>();
        }

        public IConfiguration Configuration { get; }
        private BotConfiguration BotConfig { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
	        services.Configure<LiteDbOptions>(Configuration.GetSection("LiteDbOptions"));
	        services.AddHttpClient("tgwebclient")
		        .AddTypedClient<ITelegramBotClient>(httpClient
			        => new TelegramBotClient(BotConfig.Token, httpClient));
	        services.AddMediatR(typeof(Startup).Assembly);
	        services.AddSingleton<IServiceFactory>(provider => new ServiceFactory(provider.GetService));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ICommandsFactory, CommandsFactory>();
            services.AddHostedService<ReceivingHostedService>();
            services.AddHostedService<DtekCheckingHostedService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

        }

    }
}
