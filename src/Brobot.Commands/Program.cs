using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Brobot.Commands.Services;
using Brobot.Core.Services;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Brobot.Commands
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
                    var commandsSettings = new CommandsSettings();
                    hostContext.Configuration.GetSection(nameof(CommandsSettings)).Bind(commandsSettings);

                    services.Configure<CommandsSettings>(hostContext.Configuration.GetSection(nameof(CommandsSettings)));
                    services.AddHttpClient<IBrobotService, BrobotService>(configure =>
                    {
                        configure.BaseAddress = new Uri(commandsSettings.BaseUrl);
                        configure.DefaultRequestHeaders.Add("x-api-key", commandsSettings.ApiKey);
                    });

                    services.AddHttpClient<IRandomFactService, RandomFactService>(configure =>
                    {
                        configure.BaseAddress = new Uri(commandsSettings.RandomFactBaseUrl);
                    });

                    services.AddHttpClient<IGiphyService, GiphyService>(configure =>
                    {
                        configure.BaseAddress = new Uri(commandsSettings.GiphyBaseUrl);
                    });

                    var config = new DiscordSocketConfig
                    {
                        AlwaysDownloadUsers = true
                    };
                    var client = new DiscordSocketClient(config);
                    services.AddSingleton<DiscordSocketClient>(client);
                    services.AddSingleton<CommandService>();
                    services.AddSingleton<CommandHandlingService>();
                    services.AddSingleton<Random>();
                    services.AddHostedService<CommandRefreshWorker>();
                });
    }
}
