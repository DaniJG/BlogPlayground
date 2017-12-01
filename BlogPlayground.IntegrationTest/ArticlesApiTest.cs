using BlogPlayground.IntegrationTest.Data;
using BlogPlayground.Models;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlogPlayground.IntegrationTest
{
    public class ArticlesApiTest : TestFixture
    {
        [Fact]
        public async Task GetArticles_ReturnsArticlesList()
        {
            // Act
            var response = await _client.GetAsync("/api/articles");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var articles = JsonConvert.DeserializeObject<Article[]>(responseString);
            Assert.NotStrictEqual(PredefinedData.Articles, articles);
        }

        [Fact]
        public async Task GetArticle_ReturnsSpecifiedArticle()
        {            
            // Act
            var response = await _client.GetAsync($"/api/articles/{PredefinedData.Articles[0].ArticleId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var article = JsonConvert.DeserializeObject<Article>(responseString);
            Assert.NotStrictEqual(PredefinedData.Articles[0], article);
        }        

        [Fact]
        public async Task AddArticle_ReturnsAddedArticle()
        {
            // Arrange
            await EnsureAuthenticationCookie();
            await EnsureAntiforgeryTokenHeader();
            var article = new Article { Title = "mock title", Abstract = "mock abstract", Contents = "mock contents" };

            // Act
            var contents = new StringContent(JsonConvert.SerializeObject(article), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/articles", contents);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var addedArticle = JsonConvert.DeserializeObject<Article>(responseString);
            Assert.True(addedArticle.ArticleId > 0, "Expected added article to have a valid id");
        }

        [Fact]
        public async Task DeleteConfirmation_RedirectsToList_AfterDeletingArticle()
        {
            // Arrange
            await EnsureAuthenticationCookie();
            await EnsureAntiforgeryTokenHeader();

            // Act
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"/api/articles/{PredefinedData.Articles[0].ArticleId}", UriKind.Relative),
            };
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);            
        }
    }
}
