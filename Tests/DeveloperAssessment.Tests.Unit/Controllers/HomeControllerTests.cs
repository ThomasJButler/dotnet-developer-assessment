using DeveloperAssessment.Web.Controllers;
using DeveloperAssessment.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using System;

namespace DeveloperAssessment.Tests.Unit.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _controller = new HomeController(_mockLogger.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Privacy_ReturnsViewResult()
        {
            // Arrange

            // Act
            var result = _controller.Privacy() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.ViewName.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Error_ReturnsViewWithErrorViewModel()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "TestTraceId";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error() as ViewResult;

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().BeOfType<ErrorViewModel>();
            var model = result.Model as ErrorViewModel;
            model.RequestId.Should().Be("TestTraceId");
        }

        [Fact]
        public void Error_ShowRequestId_ReturnsTrueWhenRequestIdNotEmpty()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "TestTraceId123";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error() as ViewResult;
            var model = result.Model as ErrorViewModel;

            // Assert
            model.ShowRequestId.Should().BeTrue();
        }

        [Fact]
        public void Error_ShowRequestId_ReturnsFalseWhenRequestIdEmpty()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel { RequestId = "" };

            // Act
            var showRequestId = errorViewModel.ShowRequestId;

            // Assert
            showRequestId.Should().BeFalse();
        }

        [Fact]
        public void Error_ShowRequestId_ReturnsFalseWhenRequestIdNull()
        {
            // Arrange
            var errorViewModel = new ErrorViewModel { RequestId = null };

            // Act
            var showRequestId = errorViewModel.ShowRequestId;

            // Assert
            showRequestId.Should().BeFalse();
        }

    }
}