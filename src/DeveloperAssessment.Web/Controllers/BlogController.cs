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
    }
}