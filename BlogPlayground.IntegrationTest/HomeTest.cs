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
    public class HomeTest: TestFixture
    {
        [Fact]
        public async Task Index_Get_ReturnsIndexHtmlPage()
        {
            // Act
            var response = await _client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();        
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("<title>Home Page - BlogPlayground</title>", responseString);
        }
    }
}
