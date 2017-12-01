using System;
using Xunit;
using Moq;
using BlogPlayground.Data;
using BlogPlayground.Services;
using BlogPlayground.Controllers;
using BlogPlayground.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogPlayground.Test
{
    public class ArticlesApiControllerTest
    {
        private Mock<IArticlesRepository> articlesRepoMock;
        private Mock<IRequestUserProvider> requestUserProviderMock;
        private ArticlesApiController controller;

        public ArticlesApiControllerTest()
        {
            articlesRepoMock = new Mock<IArticlesRepository>();
            requestUserProviderMock = new Mock<IRequestUserProvider>();
            controller = new ArticlesApiController(articlesRepoMock.Object, requestUserProviderMock.Object);
        }

        [Fact]
        public async Task GetArticlesTest_RetursArticlesList()
        {
            // Arrange
            var mockArticlesList = new List<Article>
            {
                new Article { Title = "mock article 1" },
                new Article { Title = "mock article 2" }
            };
            articlesRepoMock.Setup(repo => repo.GetAll()).Returns(Task.FromResult(mockArticlesList));            

            // Act
            var result = await controller.GetArticles();

            // Assert
            Assert.Equal(mockArticlesList, result);
        }

        [Fact]
        public async Task GetArticleTest_ReturnsNotFound_WhenArticleDoesNorExists()
        {
            // Arrange
            var mockId = 42;
            articlesRepoMock.Setup(repo => repo.GetOne(mockId)).Returns(Task.FromResult<Article>(null));

            // Act
            var result = await controller.GetArticle(mockId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetArticleTest_ReturnsArticle_WhenArticleExists()
        {
            // Arrange
            var mockId = 42;
            var mockArticle = new Article { Title = "mock article" };
            articlesRepoMock.Setup(repo => repo.GetOne(mockId)).Returns(Task.FromResult(mockArticle));

            // Act
            var result = await controller.GetArticle(mockId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(mockArticle, actionResult.Value);
        }
        
        [Fact]
        public async Task AddArticleTest_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockArticle = new Article { Title = "mock article" };
            controller.ModelState.AddModelError("Description", "This field is required");

            // Act
            var result = await controller.AddArticle(mockArticle);

            // Assert
            var actionResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(new SerializableError(controller.ModelState), actionResult.Value);
        }

        [Fact]
        public async Task AddArticleTest_ReturnsArticleSuccessfullyAdded()
        {
            // Arrange
            var mockArticle = new Article { Title = "mock article" };
            articlesRepoMock.Setup(repo => repo.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await controller.AddArticle(mockArticle);

            // Assert
            articlesRepoMock.Verify(repo => repo.Add(mockArticle));
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(mockArticle, actionResult.Value);
        }

        [Fact]
        public async Task AddArticleTest_SetsAuthorId_BeforeAddingArticleToRepository()
        {
            // Arrange
            var mockArticle = new Article { Title = "mock article" };
            var mockAuthorId = "mockAuthorId";
            articlesRepoMock.Setup(repo => repo.SaveChanges()).Returns(Task.CompletedTask);
            requestUserProviderMock.Setup(provider => provider.GetUserId()).Returns(mockAuthorId);

            // Act
            var result = await controller.AddArticle(mockArticle);

            // Assert
            articlesRepoMock.Verify(repo => 
                repo.Add(It.Is<Article>(article => 
                    article == mockArticle 
                    && article.AuthorId == mockAuthorId)));
        }

        [Fact]
        public async Task AddArticleTest_SetsCreatedDate_BeforeAddingArticleToRepository()
        {
            // Arrange
            var mockArticle = new Article { Title = "mock article" };
            var startTime = DateTime.Now;
            articlesRepoMock.Setup(repo => repo.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await controller.AddArticle(mockArticle);
            var endTime = DateTime.Now;

            // Assert
            articlesRepoMock.Verify(repo =>
                repo.Add(It.Is<Article>(article =>
                    article == mockArticle
                    && article.CreatedDate >= startTime
                    && article.CreatedDate <= endTime)));
        }
        
        [Fact]
        public async Task DeleteArticleTest_ReturnsNotFound_WhenArticleDoesNorExists()
        {
            // Arrange
            var mockId = 42;
            articlesRepoMock.Setup(repo => repo.GetOne(mockId)).Returns(Task.FromResult<Article>(null));

            // Act
            var result = await controller.DeleteArticle(mockId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteArticleTest_ReturnsSuccessCode_AfterRemovingArticleFromRepository()
        {
            // Arrange
            var mockId = 42;
            var mockArticle = new Article { Title = "mock article" };
            articlesRepoMock.Setup(repo => repo.GetOne(mockId)).Returns(Task.FromResult(mockArticle));
            articlesRepoMock.Setup(repo => repo.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await controller.DeleteArticle(mockId);

            // Assert
            articlesRepoMock.Verify(repo => repo.Remove(mockArticle));
            Assert.IsType<NoContentResult>(result);            
        }
    }
}
