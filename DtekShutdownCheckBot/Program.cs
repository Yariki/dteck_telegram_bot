using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DtekShutdownCheckBot
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
                {   
                    #if DEBUG
                        webBuilder.UseStartup<Startup>();
                    #else
                        var port = Environment.GetEnvironmentVariable("PORT");
                        webBuilder.UseStartup<Startup>().UseUrls("http://*:" + port);
                    #endif
                });
    }
}