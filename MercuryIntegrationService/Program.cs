using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MercuryIntegrationService
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = 
                Host.CreateDefaultBuilder(args)
                    .ConfigureLogging((context, builder) =>
                    {
                        builder.ClearProviders();
                        builder.AddDebug();
                        builder.AddSimpleConsole();
                        builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    });

            return hostBuilder;
        }
    }
}
