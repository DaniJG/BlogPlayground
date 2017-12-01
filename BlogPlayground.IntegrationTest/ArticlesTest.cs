using BlogPlayground.IntegrationTest.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace BlogPlayground.IntegrationTest
{
    public class ArticlesTest : TestFixture
    {
        [Fact]
        public async Task Index_Get_ReturnsIndexHtmlPage_ListingEveryArticle()
        {
            // Act
            var response = await _client.GetAsync("/Articles");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            foreach (var article in PredefinedData.Articles)
            {
                Assert.Contains($"<li data-articleid=\"{ article.ArticleId }\">", responseString);
            }
        }

        [Fact]
        public async Task Details_Get_ReturnsHtmlPage()
        {            
            // Act
            var response = await _client.GetAsync($"/Articles/Details/{PredefinedData.Articles[0].ArticleId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains(PredefinedData.Articles[0].Contents, responseString);
        }

        [Fact]
        public async Task Create_Get_ReturnsHtmlPage()
        {
            // Arrange
            await EnsureAuthenticationCookie();

            // Act
            var response = await _client.GetAsync("/Articles/Create");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("<h4>Create new article</h4>", responseString);
        }

        [Fact]
        public async Task Create_Post_RedirectsToList_AfterCreatingArticle()
        {
            // Arrange
            await EnsureAuthenticationCookie();
            var formData = await EnsureAntiforgeryTokenForm(new Dictionary<string, string>
            {
                { "Title", "mock title" },
                { "Abstract", "mock abstract" },
                { "Contents", "mock contents" }
            });

            // Act
            var response = await _client.PostAsync("/Articles/Create", new FormUrlEncodedContent(formData));

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            Assert.Equal("/Articles", response.Headers.Location.ToString());
        }

        [Fact]
        public async Task Delete_Get_ReturnsHtmlPage()
        {
            // Arrange
            await EnsureAuthenticationCookie();

            // Act
            var response = await _client.GetAsync($"/Articles/Delete/{PredefinedData.Articles[0].ArticleId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("<h3>Are you sure you want to delete this?</h3>", responseString);
        }

        [Fact]
        public async Task DeleteConfirmation_RedirectsToList_AfterDeletingArticle()
        {
            // Arrange
            await EnsureAuthenticationCookie();
            var formData = await EnsureAntiforgeryTokenForm();

            // Act
            var response = await _client.PostAsync($"/Articles/Delete/{PredefinedData.Articles[0].ArticleId}", new FormUrlEncodedContent(formData));

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            Assert.Equal("/Articles", response.Headers.Location.ToString());
        }
    }
}
