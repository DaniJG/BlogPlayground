using BlogPlayground.IntegrationTest.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BlogPlayground.IntegrationTest
{
    public class TestFixture: IDisposable
    {
        protected readonly TestServer _server;
        protected readonly HttpClient _client;

        public TestFixture()
        {
            // To avoid hardcoding path to project, see: https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing#integration-testing
            var integrationTestsPath = PlatformServices.Default.Application.ApplicationBasePath; // integration_tests/bin/Debug/netcoreapp2.0
            var applicationPath = Path.GetFullPath(Path.Combine(integrationTestsPath, "../../../../BlogPlayground"));

            _server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestStartup>()
                .UseContentRoot(applicationPath)
                .UseEnvironment("Development"));
            _client = _server.CreateClient();
        }

        public void Dispose()
        {
            _client.Dispose();
            _server.Dispose();
        }        
    }
}
