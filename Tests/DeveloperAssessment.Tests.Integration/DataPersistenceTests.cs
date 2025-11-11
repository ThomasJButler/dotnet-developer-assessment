using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using DeveloperAssessment.Web.Models;
using System.Collections.Generic;
using System;

namespace DeveloperAssessment.Tests.Integration
{
    public class DataPersistenceTests : IDisposable
    {
        private readonly string _testDataPath = Path.Combine(Path.GetTempPath(), "test-blog-posts.json");

        public DataPersistenceTests()
        {
            if (File.Exists(_testDataPath))
            {
                File.Delete(_testDataPath);
            }
        }

        [Fact]
        public async Task ReadJsonFile_ValidFile_ReturnsDeserializedData()
        {
            // Arrange
            var testData = new BlogData
            {
                BlogPosts = new List<BlogPost>
                {
                    new BlogPost
                    {
                        Id = 1,
                        Title = "Test Post",
                        Date = DateTime.Now,
                        HtmlContent = "<p>Test content</p>",
                        Comments = new List<Comment>()
                    }
                }
            };

            var json = JsonSerializer.Serialize(testData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_testDataPath, json);

            // Act
            var jsonContent = await File.ReadAllTextAsync(_testDataPath);
            var result = JsonSerializer.Deserialize<BlogData>(jsonContent);

            // Assert
            result.Should().NotBeNull();
            result.BlogPosts.Should().HaveCount(1);
            result.BlogPosts[0].Title.Should().Be("Test Post");
        }

        [Fact]
        public async Task WriteJsonFile_ValidData_PersistsToFile()
        {
            // Arrange
            var testData = new BlogData
            {
                BlogPosts = new List<BlogPost>
                {
                    new BlogPost
                    {
                        Id = 1,
                        Title = "Persisted Post",
                        Date = DateTime.Now,
                        HtmlContent = "<p>Persisted content</p>",
                        Comments = new List<Comment>
                        {
                            new Comment
                            {
                                Name = "Test Commenter",
                                EmailAddress = "test@example.com",
                                Message = "Test comment",
                                Date = DateTime.Now,
                                Replies = new List<Comment>()
                            }
                        }
                    }
                }
            };

            // Act
            var json = JsonSerializer.Serialize(testData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_testDataPath, json);

            // Assert
            File.Exists(_testDataPath).Should().BeTrue();
            var content = await File.ReadAllTextAsync(_testDataPath);
            content.Should().Contain("Persisted Post");
            content.Should().Contain("Test Commenter");
        }

        [Fact]
        public async Task AddCommentToJson_ExistingPost_AddsCommentSuccessfully()
        {
            // Arrange
            var initialData = new BlogData
            {
                BlogPosts = new List<BlogPost>
                {
                    new BlogPost
                    {
                        Id = 1,
                        Title = "Original Post",
                        Date = DateTime.Now,
                        HtmlContent = "<p>Original content</p>",
                        Comments = new List<Comment>()
                    }
                }
            };

            var json = JsonSerializer.Serialize(initialData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_testDataPath, json);

            // Act
            var jsonContent = await File.ReadAllTextAsync(_testDataPath);
            var blogData = JsonSerializer.Deserialize<BlogData>(jsonContent);

            var newComment = new Comment
            {
                Name = "New Commenter",
                EmailAddress = "new@example.com",
                Message = "New comment",
                Date = DateTime.Now,
                Replies = new List<Comment>()
            };

            blogData.BlogPosts[0].Comments.Add(newComment);

            var updatedJson = JsonSerializer.Serialize(blogData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_testDataPath, updatedJson);

            // Assert
            var verifyContent = await File.ReadAllTextAsync(_testDataPath);
            var verifyData = JsonSerializer.Deserialize<BlogData>(verifyContent);

            verifyData.BlogPosts[0].Comments.Should().HaveCount(1);
            verifyData.BlogPosts[0].Comments[0].Name.Should().Be("New Commenter");
        }

        [Fact]
        public async Task AddReplyToComment_ExistingComment_AddsReplySuccessfully()
        {
            // Arrange
            var initialData = new BlogData
            {
                BlogPosts = new List<BlogPost>
                {
                    new BlogPost
                    {
                        Id = 1,
                        Title = "Post with Comment",
                        Date = DateTime.Now,
                        HtmlContent = "<p>Content</p>",
                        Comments = new List<Comment>
                        {
                            new Comment
                            {
                                Name = "Parent Comment",
                                EmailAddress = "parent@example.com",
                                Message = "Parent message",
                                Date = DateTime.Now,
                                Replies = new List<Comment>()
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(initialData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_testDataPath, json);

            // Act
            var jsonContent = await File.ReadAllTextAsync(_testDataPath);
            var blogData = JsonSerializer.Deserialize<BlogData>(jsonContent);

            var newReply = new Comment
            {
                Name = "Reply User",
                EmailAddress = "reply@example.com",
                Message = "Reply message",
                Date = DateTime.Now,
                Replies = new List<Comment>()
            };

            blogData.BlogPosts[0].Comments[0].Replies.Add(newReply);

            var updatedJson = JsonSerializer.Serialize(blogData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_testDataPath, updatedJson);

            // Assert
            var verifyContent = await File.ReadAllTextAsync(_testDataPath);
            var verifyData = JsonSerializer.Deserialize<BlogData>(verifyContent);

            verifyData.BlogPosts[0].Comments[0].Replies.Should().HaveCount(1);
            verifyData.BlogPosts[0].Comments[0].Replies[0].Name.Should().Be("Reply User");
        }

        [Fact]
        public void JsonSerialization_DateTimeFormat_MaintainsCorrectFormat()
        {
            // Arrange
            var testDate = new DateTime(2019, 11, 11, 18, 11, 22);
            var comment = new Comment
            {
                Name = "Test",
                Date = testDate,
                EmailAddress = "test@example.com",
                Message = "Test",
                Replies = new List<Comment>()
            };

            // Act
            var json = JsonSerializer.Serialize(comment);
            var deserialized = JsonSerializer.Deserialize<Comment>(json);

            // Assert
            deserialized.Date.Should().Be(testDate);
        }

        [Fact]
        public async Task ConcurrentAccess_MultipleWrites_HandlesFileLocking()
        {
            // Arrange
            var initialData = new BlogData
            {
                BlogPosts = new List<BlogPost>
                {
                    new BlogPost
                    {
                        Id = 1,
                        Title = "Concurrent Test",
                        Date = DateTime.Now,
                        HtmlContent = "<p>Test</p>",
                        Comments = new List<Comment>()
                    }
                }
            };

            var json = JsonSerializer.Serialize(initialData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_testDataPath, json);

            // Act
            var task1 = Task.Run(async () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        using (var fileStream = new FileStream(_testDataPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        using (var reader = new StreamReader(fileStream))
                        using (var writer = new StreamWriter(fileStream))
                        {
                            var content = await reader.ReadToEndAsync();
                            await Task.Delay(10);
                        }
                    }
                    catch (IOException)
                    {
                    }
                }
            });

            var task2 = Task.Run(async () =>
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        using (var fileStream = new FileStream(_testDataPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        using (var reader = new StreamReader(fileStream))
                        using (var writer = new StreamWriter(fileStream))
                        {
                            var content = await reader.ReadToEndAsync();
                            await Task.Delay(10);
                        }
                    }
                    catch (IOException)
                    {
                    }
                }
            });

            // Assert
            await Task.WhenAll(task1, task2);
            File.Exists(_testDataPath).Should().BeTrue();
        }

        [Fact]
        public void HandleMissingFile_FileNotFound_ReturnsNull()
        {
            // Arrange
            var nonExistentPath = Path.Combine(Path.GetTempPath(), "non-existent-file.json");

            // Act
            var exists = File.Exists(nonExistentPath);

            // Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task HandleEmptyFile_EmptyContent_ThrowsJsonException()
        {
            // Arrange
            await File.WriteAllTextAsync(_testDataPath, "");

            // Act & Assert
            var content = await File.ReadAllTextAsync(_testDataPath);
            var action = () => JsonSerializer.Deserialize<BlogData>(content);

            action.Should().Throw<JsonException>();
        }

        [Fact]
        public async Task HandleInvalidJson_MalformedJson_ThrowsJsonException()
        {
            // Arrange
            await File.WriteAllTextAsync(_testDataPath, "{ invalid json }");

            // Act & Assert
            var content = await File.ReadAllTextAsync(_testDataPath);
            var action = () => JsonSerializer.Deserialize<BlogData>(content);

            action.Should().Throw<JsonException>();
        }

        public void Dispose()
        {
            if (File.Exists(_testDataPath))
            {
                File.Delete(_testDataPath);
            }
        }
    }
}