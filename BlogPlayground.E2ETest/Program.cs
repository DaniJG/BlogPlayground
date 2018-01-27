using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

namespace BlogPlayground.E2ETest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            // To avoid hardcoding path to project, see: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing#integration-testing
            var integrationTestsPath = PlatformServices.Default.Application.ApplicationBasePath; // e2e_tests/bin/Debug/netcoreapp2.0
            var applicationPath = Path.GetFullPath(Path.Combine(integrationTestsPath, "../../../../BlogPlayground"));

            return WebHost.CreateDefaultBuilder(args)
                  .UseStartup<TestStartup>()
                  .UseContentRoot(applicationPath)
                  .UseEnvironment("Development")
                  .Build();
        }
    }
}
