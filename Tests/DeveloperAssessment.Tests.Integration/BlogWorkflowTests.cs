using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using DeveloperAssessment.Web;

namespace DeveloperAssessment.Tests.Integration
{
    public class BlogWorkflowTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public BlogWorkflowTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Index")]
        [InlineData("/Home/Privacy")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.Content.Headers.ContentType.ToString().Should().Contain("text/html");
        }

        [Theory]
        [InlineData("/blog/1")]
        [InlineData("/blog/2")]
        [InlineData("/blog/3")]
        public async Task Get_BlogPostById_ReturnsSuccess(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("/blog/999")]
        [InlineData("/blog/0")]
        [InlineData("/blog/-1")]
        public async Task Get_BlogPostWithInvalidId_ReturnsNotFound(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_BlogPost1_ContainsTitle()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/blog/1");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            content.Should().Contain("Top 5 Considerations for a Content-First Design Approach");
        }

        [Fact]
        public async Task Get_BlogPost2_ContainsTitle()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/blog/2");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            content.Should().Contain("Introducing Ben");
        }

        [Fact]
        public async Task Get_BlogPost3_ContainsTitle()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/blog/3");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            content.Should().Contain("Highlights from Kentico Connection Brno");
        }

        [Fact]
        public async Task Get_BlogPost_ContainsComments()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/blog/1");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            content.Should().Contain("Steven Barker");
            content.Should().Contain("Chetan Mistry");
            content.Should().Contain("Chris Grey");
        }

        [Fact]
        public async Task Get_HomePage_ContainsExpectedContent()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            content.Should().Contain("Junior .NET Web Developer Assessment");
            content.Should().Contain("Thomas Butler");
        }

        [Fact]
        public async Task Get_NavigationLinks_ArePresent()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            content.Should().Contain("Blog 1");
            content.Should().Contain("Blog 2");
            content.Should().Contain("Blog 3");
        }
    }
}
