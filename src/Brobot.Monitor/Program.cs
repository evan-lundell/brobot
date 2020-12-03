using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Brobot.Core.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Brobot.Monitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(config =>
                {
                    var development = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
                    var isDev = string.IsNullOrWhiteSpace(development) || development.ToLower() == "development";

                    if (isDev)
                    {
                        config.AddUserSecrets(Assembly.GetExecutingAssembly());
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var responsesSettings = new MonitorSettings();
                    hostContext.Configuration.GetSection(nameof(MonitorSettings)).Bind(responsesSettings);

                    services.Configure<MonitorSettings>(hostContext.Configuration.GetSection(nameof(MonitorSettings)));
                    services.AddHttpClient<IBrobotService, BrobotService>(configure =>
                    {
                        configure.BaseAddress = new Uri(responsesSettings.BaseUrl);
                        configure.DefaultRequestHeaders.Add("x-api-key", responsesSettings.ApiKey);
                    });

                    var discordConfig = new DiscordSocketConfig
                    {
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 100
                    };
                    services.AddSingleton(new DiscordSocketClient(discordConfig));
                    services.AddHostedService<MonitorWorker>();
                });
    }
}
