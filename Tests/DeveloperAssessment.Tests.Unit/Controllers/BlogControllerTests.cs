using DeveloperAssessment.Web.Controllers;
using DeveloperAssessment.Web.Models;
using DeveloperAssessment.Web.Models.ViewModels;
using DeveloperAssessment.Tests.Unit.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace DeveloperAssessment.Tests.Unit.Controllers
{
    public class BlogControllerTests
    {
        private readonly Mock<IWebHostEnvironment> _mockHostEnvironment;
        private readonly BlogController _controller;

        public BlogControllerTests()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "src", "DeveloperAssessment.Web"));

            _mockHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockHostEnvironment.Setup(x => x.ContentRootPath).Returns(projectRoot);
            _controller = new BlogController(_mockHostEnvironment.Object);
        }

        [Fact]
        public void Details_ValidBlogId_ReturnsViewWithCorrectModel()
        {
            // Arrange
            int validBlogId = 1;

            // Act
            var result = _controller.Details(validBlogId) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();
            result.Model.Should().BeOfType<BlogPostViewModel>();
        }

        [Fact]
        public void Details_InvalidBlogId_ReturnsNotFound()
        {
            // Arrange
            int invalidBlogId = 999;

            // Act
            var result = _controller.Details(invalidBlogId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Details_ZeroBlogId_ReturnsNotFound()
        {
            // Arrange
            int zeroBlogId = 0;

            // Act
            var result = _controller.Details(zeroBlogId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Details_NegativeBlogId_ReturnsNotFound()
        {
            // Arrange
            int negativeBlogId = -1;

            // Act
            var result = _controller.Details(negativeBlogId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void AddComment_ValidModel_RedirectsToDetails()
        {
            // Arrange
            var validComment = TestDataHelper.GetValidCommentViewModel();

            // Act
            var result = _controller.AddComment(validComment.BlogPostId, validComment) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Details");
            result.RouteValues["id"].Should().Be(validComment.BlogPostId);
        }

        [Fact]
        public void AddComment_InvalidModel_ReturnsView()
        {
            // Arrange
            var invalidComment = TestDataHelper.GetInvalidCommentViewModel();
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = _controller.AddComment(1, invalidComment) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeOfType<BlogPostViewModel>();
        }

        [Fact]
        public void AddComment_NullModel_ReturnsView()
        {
            // Arrange
            AddCommentViewModel nullComment = null;
            _controller.ModelState.AddModelError("", "Model is null");

            // Act
            var result = _controller.AddComment(1, nullComment) as ViewResult;

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void AddReply_ValidReply_RedirectsToDetails()
        {
            // Arrange
            var validReply = TestDataHelper.GetValidCommentViewModel();
            string parentName = "Test Parent";

            // Act
            var result = _controller.AddReply(validReply.BlogPostId, parentName, validReply) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Details");
            result.RouteValues["id"].Should().Be(validReply.BlogPostId);
        }

        [Fact]
        public void AddReply_InvalidModel_ReturnsView()
        {
            // Arrange
            var invalidReply = TestDataHelper.GetInvalidCommentViewModel();
            string parentName = "Test Parent";
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = _controller.AddReply(1, parentName, invalidReply) as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeOfType<BlogPostViewModel>();
        }

        [Fact]
        public void AddReply_EmptyParentName_StillProcesses()
        {
            // Arrange
            var validReply = TestDataHelper.GetValidCommentViewModel();
            string emptyParentName = "";

            // Act
            var result = _controller.AddReply(validReply.BlogPostId, emptyParentName, validReply) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Details");
        }

        [Fact]
        public void AddReply_NullParentName_HandlesGracefully()
        {
            // Arrange
            var validReply = TestDataHelper.GetValidCommentViewModel();
            string nullParentName = null;

            // Act
            var result = _controller.AddReply(validReply.BlogPostId, nullParentName, validReply) as RedirectToActionResult;

            // Assert
            result.Should().NotBeNull();
            result.ActionName.Should().Be("Details");
        }
    }
}