using System.Text;
using DtekShutdownCheckBot.Commands;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Shared.Entities;
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

            services.AddControllersWithViews();
            services.AddRazorPages();

	        services.Configure<LiteDbOptions>(Configuration.GetSection("LiteDbOptions"));
	        services.AddHttpClient("tgwebclient")
		        .AddTypedClient<ITelegramBotClient>(httpClient
			        => new TelegramBotClient(BotConfig.Token, httpClient));
	        services.AddMediatR(typeof(Startup).Assembly);
	        services.AddSingleton<IServiceFactory>(provider => new ServiceFactory(provider.GetService));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ICommandsFactory, CommandsFactory>();
            services.AddHostedService<TelegramBotHostedService>();
            services.AddHostedService<DtekCheckingHostedService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }

    }
}
