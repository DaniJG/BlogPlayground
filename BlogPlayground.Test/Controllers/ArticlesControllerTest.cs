using System;
using Xunit;
using Moq;
using BlogPlayground.Data;
using BlogPlayground.Services;
using BlogPlayground.Controllers;
using System.Collections.Generic;
using BlogPlayground.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BlogPlayground.Test
{
    public class ArticlesControllerTest
    {
        private Mock<IArticlesRepository> articlesRepoMock;
        private Mock<IRequestUserProvider> requestUserProviderMock;
        private ArticlesController controller;

        public ArticlesControllerTest()
        {
            articlesRepoMock = new Mock<IArticlesRepository>();
            requestUserProviderMock = new Mock<IRequestUserProvider>();
            controller = new ArticlesController(articlesRepoMock.Object, requestUserProviderMock.Object);
        }

        [Fact]
        public async Task IndexTest_ReturnsViewWithArticlesList()
        {
            // Arrange
            var mockArticlesList = new List<Article>
            {
                new Article { Title = "mock article 1" },
                new Article { Title = "mock article 2" }
            };
            articlesRepoMock.Setup(repo => repo.GetAll()).Returns(Task.FromResult(mockArticlesList));            

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Article>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task DetailsTest_ReturnsNotFound_WhenNoIdProvided()
        {
            // Act
            var result = await controller.Details(null);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsTest_ReturnsNotFound_WhenArticleDoesNorExists()
        {
            // Arrange
            var mockId = 42;
            articlesRepoMock.Setup(repo => repo.GetOne(mockId)).Returns(Task.FromResult<Article>(null));

            // Act
            var result = await controller.Details(mockId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DetailsTest_ReturnsDetailsView_WhenArticleExists()
        {
            // Arrange
            var mockId = 42;
            var mockArticle = new Article { Title = "mock article" };
            articlesRepoMock.Setup(repo => repo.GetOne(mockId)).Returns(Task.FromResult(mockArticle));

            // Act
            var result = await controller.Details(mockId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(mockArticle, viewResult.ViewData.Model);
        }

        [Fact]
        public void CreateTest_Get_ReturnsView()
        {
            // Act
            var result = controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task CreateTest_Post_ReturnsCreateView_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockArticle = new Article { Title = "mock article" };
            controller.ModelState.AddModelError("Description", "This field is required");

            // Act
            var result = await controller.Create(mockArticle);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(mockArticle, viewResult.ViewData.Model);
        }

        [Fact]
        public async Task CreateTest_Post_AddsArticleFromRepository_AndRedirectsToIndex()
        {
            // Arrange
            var mockArticle = new Article { Title = "mock article" };
            articlesRepoMock.Setup(repo => repo.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await controller.Create(mockArticle);

            // Assert
            articlesRepoMock.Verify(repo => repo.Add(mockArticle));
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public async Task CreateTest_Post_SetsAuthorId_BeforeAddingArticleToRepository()
        {
            // Arrange
            var mockArticle = new Article { Title = "mock article" };
            var mockAuthorId = "mockAuthorId";
            articlesRepoMock.Setup(repo => repo.SaveChanges()).Returns(Task.CompletedTask);
            requestUserProviderMock.Setup(provider => provider.GetUserId()).Returns(mockAuthorId);

            // Act
            var result = await controller.Create(mockArticle);

            // Assert
            articlesRepoMock.Verify(repo => 
                repo.Add(It.Is<Article>(article => 
                    article == mockArticle 
                    && article.AuthorId == mockAuthorId)));
        }

        [Fact]
        public async Task CreateTest_Post_SetsCreatedDate_BeforeAddingArticleToRepository()
        {
            // Arrange
            var mockArticle = new Article { Title = "mock article" };
            var startTime = DateTime.Now;
            articlesRepoMock.Setup(repo => repo.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await controller.Create(mockArticle);
            var endTime = DateTime.Now;

            // Assert
            articlesRepoMock.Verify(repo =>
                repo.Add(It.Is<Article>(article =>
                    article == mockArticle
                    && article.CreatedDate >= startTime
                    && article.CreatedDate <= endTime)));
        }

        [Fact]
        public async Task DeleteTest_ReturnsNotFound_WhenNoIdProvided()
        {
            // Act
            var result = await controller.Delete(null);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTest_ReturnsNotFound_WhenArticleDoesNorExists()
        {
            // Arrange
            var mockId = 42;
            articlesRepoMock.Setup(repo => repo.GetOne(mockId)).Returns(Task.FromResult<Article>(null));

            // Act
            var result = await controller.Delete(mockId);

            // Assert
            var viewResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTest_ReturnsDeleteView_WhenArticleExists()
        {
            // Arrange
            var mockId = 42;
            var mockArticle = new Article { Title = "mock article" };
            articlesRepoMock.Setup(repo => repo.GetOne(mockId)).Returns(Task.FromResult(mockArticle));

            // Act
            var result = await controller.Delete(mockId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(mockArticle, viewResult.ViewData.Model);
        }

        [Fact]
        public async Task DeleteConfirmedTest_RemovesArticleFromRepository_AndRedirectsToIndex()
        {
            // Arrange
            var mockId = 42;
            var mockArticle = new Article { Title = "mock article" };
            articlesRepoMock.Setup(repo => repo.GetOne(mockId)).Returns(Task.FromResult(mockArticle));
            articlesRepoMock.Setup(repo => repo.SaveChanges()).Returns(Task.CompletedTask);

            // Act
            var result = await controller.DeleteConfirmed(mockId);

            // Assert
            articlesRepoMock.Verify(repo => repo.Remove(mockArticle));
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", viewResult.ActionName);
        }
    }
}
