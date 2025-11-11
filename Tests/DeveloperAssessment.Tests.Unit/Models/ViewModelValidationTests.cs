using DeveloperAssessment.Web.Models;
using DeveloperAssessment.Web.Models.ViewModels;
using DeveloperAssessment.Tests.Unit.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;

namespace DeveloperAssessment.Tests.Unit.Models
{
    public class ViewModelValidationTests
    {
        [Fact]
        public void AddCommentViewModel_ValidModel_PassesValidation()
        {
            // Arrange
            var model = TestDataHelper.GetValidCommentViewModel();
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeTrue();
            results.Should().BeEmpty();
        }

        [Fact]
        public void AddCommentViewModel_EmptyName_FailsValidation()
        {
            // Arrange
            var model = new AddCommentViewModel
            {
                Name = "",
                EmailAddress = "test@example.com",
                Message = "Test message",
                BlogPostId = 1
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().ContainSingle(r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void AddCommentViewModel_NullName_FailsValidation()
        {
            // Arrange
            var model = new AddCommentViewModel
            {
                Name = null,
                EmailAddress = "test@example.com",
                Message = "Test message",
                BlogPostId = 1
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().ContainSingle(r => r.MemberNames.Contains("Name"));
        }

        [Fact]
        public void AddCommentViewModel_InvalidEmailFormat_FailsValidation()
        {
            // Arrange
            var model = new AddCommentViewModel
            {
                Name = "Test User",
                EmailAddress = "invalid-email",
                Message = "Test message",
                BlogPostId = 1
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().ContainSingle(r => r.MemberNames.Contains("EmailAddress"));
        }

        [Fact]
        public void AddCommentViewModel_EmptyEmail_FailsValidation()
        {
            // Arrange
            var model = new AddCommentViewModel
            {
                Name = "Test User",
                EmailAddress = "",
                Message = "Test message",
                BlogPostId = 1
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().ContainSingle(r => r.MemberNames.Contains("EmailAddress"));
        }

        [Fact]
        public void AddCommentViewModel_EmptyMessage_FailsValidation()
        {
            // Arrange
            var model = new AddCommentViewModel
            {
                Name = "Test User",
                EmailAddress = "test@example.com",
                Message = "",
                BlogPostId = 1
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().ContainSingle(r => r.MemberNames.Contains("Message"));
        }

        [Fact]
        public void AddCommentViewModel_NullMessage_FailsValidation()
        {
            // Arrange
            var model = new AddCommentViewModel
            {
                Name = "Test User",
                EmailAddress = "test@example.com",
                Message = null,
                BlogPostId = 1
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().ContainSingle(r => r.MemberNames.Contains("Message"));
        }

        [Fact]
        public void AddCommentViewModel_ValidEmailFormats_PassValidation()
        {
            // Arrange
            var validEmails = new[]
            {
                "test@example.com",
                "user.name@domain.co.uk",
                "firstname+lastname@example.org",
                "test123@subdomain.example.com"
            };

            foreach (var email in validEmails)
            {
                var model = new AddCommentViewModel
                {
                    Name = "Test User",
                    EmailAddress = email,
                    Message = "Test message",
                    BlogPostId = 1
                };
                var context = new ValidationContext(model);
                var results = new List<ValidationResult>();

                // Act
                var isValid = Validator.TryValidateObject(model, context, results, true);

                // Assert
                isValid.Should().BeTrue($"Email {email} should be valid");
                results.Should().BeEmpty();
            }
        }

        [Fact]
        public void AddCommentViewModel_AllFieldsEmpty_FailsMultipleValidations()
        {
            // Arrange
            var model = new AddCommentViewModel
            {
                Name = "",
                EmailAddress = "",
                Message = "",
                BlogPostId = 1
            };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            isValid.Should().BeFalse();
            results.Should().HaveCount(3);
            results.Should().Contain(r => r.MemberNames.Contains("Name"));
            results.Should().Contain(r => r.MemberNames.Contains("EmailAddress"));
            results.Should().Contain(r => r.MemberNames.Contains("Message"));
        }

        [Fact]
        public void BlogPostViewModel_Properties_CanBeSet()
        {
            // Arrange
            var blogPost = TestDataHelper.GetSampleBlogPost(1);

            // Act
            var viewModel = new BlogPostViewModel
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Date = blogPost.Date,
                Image = blogPost.Image,
                HtmlContent = blogPost.HtmlContent,
                Comments = blogPost.Comments
            };

            // Assert
            viewModel.Id.Should().Be(1);
            viewModel.Title.Should().Be("Test Blog Post 1");
            viewModel.Date.Should().Be(blogPost.Date);
            viewModel.Image.Should().Contain("picsum.photos");
            viewModel.HtmlContent.Should().Contain("test content");
            viewModel.Comments.Should().NotBeNull();
            viewModel.Comments.Should().HaveCount(2);
        }
    }
}