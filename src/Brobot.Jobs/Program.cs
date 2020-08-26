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

namespace Brobot.Jobs
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
                    var jobsSettings = new JobsSettings();
                    hostContext.Configuration.GetSection(nameof(JobsSettings)).Bind(jobsSettings);

                    services.Configure<JobsSettings>(hostContext.Configuration.GetSection(nameof(JobsSettings)));
                    services.AddHttpClient<IBrobotService, BrobotService>(configure =>
                    {
                        configure.BaseAddress = new Uri(jobsSettings.BaseUrl);
                        configure.DefaultRequestHeaders.Add("x-api-key", jobsSettings.ApiKey);
                    });

                    services.AddSingleton<DiscordSocketClient>();
                    services.AddHostedService<JobsWorker>();
                });
    }
}
