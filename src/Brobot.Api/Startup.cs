using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Brobot.Api.Authentication;
using Brobot.Api.Contexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Brobot.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<BrobotDbContext>(builder => builder.UseNpgsql(Configuration.GetConnectionString("Default")));
            services.AddDbContext<AuthenticationDbContext>(builder => builder.UseNpgsql(Configuration.GetConnectionString("Default")));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
            })
            .AddApiKeySupport(options => { });

            services.AddScoped<IGetApiKeyQuery, DbGetApiKeyQuery>();
            services.AddAutoMapper(typeof(Startup));
            services.AddSingleton<Random>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
            IWebHostEnvironment env, 
            BrobotDbContext brobotDbContext, 
            AuthenticationDbContext authenticationDbContext, 
            ILogger<Startup> logger,
            IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            int retryCount = 0;

            while (!brobotDbContext.Database.CanConnect() || !authenticationDbContext.Database.CanConnect())
            {
                if (retryCount == 5)
                {
                    logger.LogError("Unabled to connect to database");
                    applicationLifetime.StopApplication();
                }
            }
            brobotDbContext.Database.Migrate();
            authenticationDbContext.Database.Migrate();
        }
    }
}
