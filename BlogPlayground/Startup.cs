using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using BlogPlayground.Data;
using BlogPlayground.Models;
using BlogPlayground.Services;

namespace BlogPlayground
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
            // Add framework services.
            ConfigureDatabase(services);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddGoogle(options => 
                {
                    options.ClientId = Configuration["GoogleClientId"] ?? "MissingClientId";
                    options.ClientSecret = Configuration["GoogleClientSecret"] ?? "MissingClientSecret";
                    options.SaveTokens = true; // So we get the access token and can use it later to retrieve the user profile including its picture
                    //options.CallbackPath = "/signin-google" DEFAULT VALUE
                }); 

            services.AddMvc();
            services.AddResponseCompression();

            //Needed for accessing action context in the tag helpers
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Add application repositories as scoped dependencies so they are shared per every request.
            services.AddScoped<IArticlesRepository, ArticlesRepository>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IGooglePictureLocator, GooglePictureLocator>();
            services.AddTransient<IRequestUserProvider, RequestUserProvider>();
        }

        public virtual void ConfigureDatabase(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
