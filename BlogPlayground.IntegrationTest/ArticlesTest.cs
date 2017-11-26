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
        protected static string AUTHENTICATION_COOKIE = ".AspNetCore.Identity.";
        protected static string ANTIFORGERY_COOKIE = ".AspNetCore.AntiForgery.";
        protected static string ANTIFORGERY_TOKEN_FORM = "__RequestVerificationToken";
        protected static Regex AntiforgeryFormFieldRegex = new Regex(@"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");

        protected SetCookieHeaderValue _authenticationCookie;
        protected SetCookieHeaderValue _antiforgeryCookie;
        protected string _antiforgeryToken;

        public async Task<Dictionary<string, string>> EnsureAntiforgeryToken(Dictionary<string, string> formData = null)
        {
            if (formData == null) formData = new Dictionary<string, string>();

            if (_antiforgeryToken == null)
            {
                var response = await _client.GetAsync("/Account/Login");
                response.EnsureSuccessStatusCode();
                if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
                {
                    _antiforgeryCookie = SetCookieHeaderValue.ParseList(values.ToList()).SingleOrDefault(c => c.Name.StartsWith(ANTIFORGERY_COOKIE, StringComparison.InvariantCultureIgnoreCase));
                }
                Assert.NotNull(_antiforgeryCookie);
                _client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(_antiforgeryCookie.Name, _antiforgeryCookie.Value).ToString());

                var responseHtml = await response.Content.ReadAsStringAsync();
                var match = AntiforgeryFormFieldRegex.Match(responseHtml);
                _antiforgeryToken = match.Success ? match.Groups[1].Captures[0].Value : null;
                Assert.NotNull(_antiforgeryToken);
            }

            formData.Add(ANTIFORGERY_TOKEN_FORM, _antiforgeryToken);
            return formData;
        }

        public async Task EnsureAuthenticationCookie()
        {
            if (_authenticationCookie != null) return;

            var formData = await EnsureAntiforgeryToken(new Dictionary<string, string>
            {
                { "Email", PredefinedData.Profiles[0].Email },
                { "Password", PredefinedData.Password }
            });        
            var request = new HttpRequestMessage(HttpMethod.Post, "/Account/Login") { Content = new FormUrlEncodedContent(formData) };
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                _authenticationCookie = SetCookieHeaderValue.ParseList(values.ToList()).SingleOrDefault(c => c.Name.StartsWith(AUTHENTICATION_COOKIE, StringComparison.InvariantCultureIgnoreCase));
            }
            Assert.NotNull(_authenticationCookie);
            _client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(_authenticationCookie.Name, _authenticationCookie.Value).ToString());

            // The current pair of antiforgery cookie-token is not valid anymore
            // Since the tokens are generated based on the authenticated user!
            // We need a new token after authentication (The cookie can stay the same)
            _antiforgeryToken = null;
        }
        

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
            var formData = await EnsureAntiforgeryToken(new Dictionary<string, string>
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
            var formData = await EnsureAntiforgeryToken();

            // Act
            var response = await _client.PostAsync($"/Articles/Delete/{PredefinedData.Articles[0].ArticleId}", new FormUrlEncodedContent(formData));

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            Assert.Equal("/Articles", response.Headers.Location.ToString());
        }
    }
}
