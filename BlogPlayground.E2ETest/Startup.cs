using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using BlogPlayground.IntegrationTest.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;
using BlogPlayground.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogPlayground.E2ETest
{
    public class TestStartup: BlogPlayground.Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureDatabase(IServiceCollection services)
        {
            // Replace default database connection with SQLite in memory
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connection = new SqliteConnection(connectionStringBuilder.ToString());
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));
            
            // Register the database seeder
            services.AddTransient<DatabaseSeeder>();
        }

        public override void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Perform all the configuration in the base class
            base.Configure(app, env, loggerFactory);

            // Now create and seed the database
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>())
            {
                dbContext.Database.OpenConnection();
                dbContext.Database.EnsureCreated();

                var seeder = serviceScope.ServiceProvider.GetService<DatabaseSeeder>();
                seeder.Seed().Wait();
            }
        }
    }
}
