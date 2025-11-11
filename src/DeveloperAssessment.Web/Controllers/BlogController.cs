using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using DeveloperAssessment.Web.Models;
using DeveloperAssessment.Web.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DeveloperAssessment.Web.Controllers
{
    // blog controller - handles all blog-related requests & json data persistence
    // uses iwebhostenvironment to locate data files & json serialisation for storage
    public class BlogController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public BlogController(IWebHostEnvironment env)
        {
            _env = env;
        }

        // helper to get the json file path - uses contentrootpath for correct location
        private string GetDataPath()
        {
            return Path.Combine(_env.ContentRootPath, "Data", "Blog-Posts.json");
        }

        // helper to read blog data - deserialises json with case-insensitive matching
        private BlogData GetBlogData()
        {
            var json = System.IO.File.ReadAllText(GetDataPath());

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<BlogData>(json, options);
        }

        // helper to save blog data - serialises with indentation & camelcase for readability
        private void SaveBlogData(BlogData data)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(data, options);
            System.IO.File.WriteAllText(GetDataPath(), json);
        }

        // get: /blog/1 - displays specific blog post by id
        // returns 404 if post not found, otherwise shows details view
        [HttpGet("/blog/{id}")]
        public IActionResult Details(int id)
        {
            var data = GetBlogData();
            var blogPost = data.BlogPosts?.FirstOrDefault(p => p.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            var viewModel = new BlogPostViewModel
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Date = blogPost.Date,
                Image = blogPost.Image,
                HtmlContent = blogPost.HtmlContent,
                Comments = blogPost.Comments ?? new List<Comment>()
            };

            return View(viewModel);
        }

        // post: /blog/1/comment - adds new comment to blog post
        // validates model, saves to json & redirects to prevent double-submit
        [HttpPost("/blog/{id}/comment")]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int id, AddCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // show the page again with validation errors
                return Details(id);
            }

            var data = GetBlogData();
            var blogPost = data.BlogPosts?.FirstOrDefault(p => p.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            // make sure comments list exists
            if (blogPost.Comments == null)
                blogPost.Comments = new List<Comment>();

            // add the new comment
            var comment = new Comment
            {
                Name = model.Name,
                EmailAddress = model.EmailAddress,
                Message = model.Message,
                Date = DateTime.UtcNow,
                Replies = new List<Comment>()
            };

            blogPost.Comments.Add(comment);

            // save back to file
            SaveBlogData(data);

            // redirect to prevent double-submit
            return RedirectToAction("Details", new { id });
        }

        // post: /blog/1/reply (for exercise 4) - adds reply to existing comment
        // finds parent comment by name & nests reply underneath
        [HttpPost("/blog/{id}/reply")]
        [ValidateAntiForgeryToken]
        public IActionResult AddReply(int id, string parentName, AddCommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Details(id);
            }

            var data = GetBlogData();
            var blogPost = data.BlogPosts?.FirstOrDefault(p => p.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            // find parent comment by name (simple approach)
            var parentComment = FindCommentByName(blogPost.Comments, parentName);

            if (parentComment != null)
            {
                if (parentComment.Replies == null)
                    parentComment.Replies = new List<Comment>();

                var reply = new Comment
                {
                    Name = model.Name,
                    EmailAddress = model.EmailAddress,
                    Message = model.Message,
                    Date = DateTime.UtcNow,
                    Replies = new List<Comment>()
                };

                parentComment.Replies.Add(reply);
                SaveBlogData(data);
            }

            return RedirectToAction("Details", new { id });
        }

        // simple helper to find comment by name - recursive search through comment tree
        private Comment FindCommentByName(List<Comment> comments, string name)
        {
            if (comments == null) return null;

            foreach (var comment in comments)
            {
                if (comment.Name == name)
                    return comment;

                // check in replies too
                var found = FindCommentByName(comment.Replies, name);
                if (found != null)
                    return found;
            }

            return null;
        }
    }
}