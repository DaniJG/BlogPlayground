using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BlogPlayground.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using BlogPlayground.IntegrationTest.Data;

namespace BlogPlayground.IntegrationTest
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureDatabase(IServiceCollection services)
        {
            // Replace default database connection with In-Memory database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("blogplayground_test_db"));

            // Register the database seeder
            services.AddTransient<DatabaseSeeder>();
        }

        public override void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Perform all the configuration in the base class
            base.Configure(app, env, loggerFactory);

            // Now seed the database
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var seeder = serviceScope.ServiceProvider.GetService<DatabaseSeeder>();
                seeder.Seed().Wait();
            }
        }
    }
}
