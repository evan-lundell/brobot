using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Brobot.Core.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Brobot.Sync
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
                    var syncSettings = new SyncSettings();
                    hostContext.Configuration.GetSection("SyncSettings").Bind(syncSettings);

                    services.Configure<SyncSettings>(hostContext.Configuration.GetSection("SyncSettings"));
                    services.AddHttpClient<IBrobotService, BrobotService>(configure =>
                    {
                        configure.BaseAddress = new Uri(syncSettings.BaseUrl);
                        configure.DefaultRequestHeaders.Add("x-api-key", syncSettings.ApiKey);
                    });

                    var config = new DiscordSocketConfig
                    {
                        AlwaysDownloadUsers = true
                    };
                    var client = new DiscordSocketClient(config);
                    services.AddSingleton<DiscordSocketClient>(client);

                    services.AddHostedService<SyncWorker>();
                });
    }
}
