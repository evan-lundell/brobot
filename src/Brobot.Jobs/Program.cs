using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Amazon.Lambda;
using Brobot.Core.Services;
using Brobot.Jobs.Services;
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
            var host = CreateHostBuilder(args).Build();
            if (args.Contains("-e"))
            {
                if (args.Length != 2 || !int.TryParse(args[1], out int jobId))
                {
                    Console.WriteLine("Invalid arguments");
                    return;
                }
                var executor = host.Services.GetRequiredService<JobExecutor>();
                executor.Execute(jobId).GetAwaiter().GetResult();
            }
            else
            {
                host.Run();
            }
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

                    services.AddHttpClient<ITwitterService, TwitterService>(configure =>
                    {
                        configure.BaseAddress = new Uri(jobsSettings.TwitterApiBaseUrl);
                        configure.DefaultRequestHeaders.Add("Authorization", $"Bearer {jobsSettings.TwitterBearerToken}");
                    });

                    var config = new DiscordSocketConfig
                    {
                        AlwaysDownloadUsers = true
                    };
                    var client = new DiscordSocketClient(config);
                    services.AddSingleton<DiscordSocketClient>(client);

                    var lambdaClient = new AmazonLambdaClient(jobsSettings.AwsAccessKey, jobsSettings.AwsSecretKey, Amazon.RegionEndpoint.USEast2);
                    services.AddSingleton(lambdaClient);
                    
                    if (args.Contains("-e"))
                    {
                        services.AddSingleton<JobExecutor>();
                    }
                    else
                    {
                        services.AddHostedService<JobsWorker>();
                    }
                });
    }
}
