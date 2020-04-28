using Brobot.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Brobot.Sync
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var syncService = services.GetRequiredService<SyncService>();
                await syncService.RunAsync();
            }
        }

        private ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<SyncService>();
            services.AddHttpClient<IBrobotService, BrobotService>(client =>
            {
                client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("BROBOT_BASEURL"));
                client.DefaultRequestHeaders.Add("x-api-key", Environment.GetEnvironmentVariable("BROBOT_APIKEY"));
            });
            return services.BuildServiceProvider();
        }
    }
}
