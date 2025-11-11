using DeveloperAssessment.Web.Models;
using DeveloperAssessment.Web.Models.ViewModels;
using System;
using System.Collections.Generic;

namespace DeveloperAssessment.Tests.Unit.Helpers
{
    public static class TestDataHelper
    {
        public static BlogData GetSampleBlogData()
        {
            return new BlogData
            {
                BlogPosts = new List<BlogPost>
                {
                    GetSampleBlogPost(1),
                    GetSampleBlogPost(2),
                    GetSampleBlogPost(3)
                }
            };
        }

        public static BlogPost GetSampleBlogPost(int id)
        {
            return new BlogPost
            {
                Id = id,
                Title = $"Test Blog Post {id}",
                Date = new DateTime(2019, 11, 11, 18, 11, 22),
                Image = $"https://picsum.photos/id/{id}/800/450",
                HtmlContent = $"<p>This is test content for blog post {id}.</p>",
                Comments = GetSampleComments()
            };
        }

        public static List<Comment> GetSampleComments()
        {
            return new List<Comment>
            {
                new Comment
                {
                    Name = "Test User 1",
                    Date = DateTime.Now.AddDays(-2),
                    EmailAddress = "test1@example.com",
                    Message = "This is a test comment.",
                    Replies = new List<Comment>()
                },
                new Comment
                {
                    Name = "Test User 2",
                    Date = DateTime.Now.AddDays(-1),
                    EmailAddress = "test2@example.com",
                    Message = "This is another test comment.",
                    Replies = new List<Comment>
                    {
                        new Comment
                        {
                            Name = "Test User 3",
                            Date = DateTime.Now,
                            EmailAddress = "test3@example.com",
                            Message = "This is a reply to the second comment.",
                            Replies = new List<Comment>()
                        }
                    }
                }
            };
        }

        public static AddCommentViewModel GetValidCommentViewModel()
        {
            return new AddCommentViewModel
            {
                Name = "Test Commenter",
                EmailAddress = "test@example.com",
                Message = "This is a valid test comment.",
                BlogPostId = 1
            };
        }

        public static AddCommentViewModel GetInvalidCommentViewModel()
        {
            return new AddCommentViewModel
            {
                Name = "",
                EmailAddress = "invalid-email",
                Message = "",
                BlogPostId = 0
            };
        }
    }
}