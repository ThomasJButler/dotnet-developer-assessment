using System;
using System.Collections.Generic;

namespace DeveloperAssessment.Web.Models
{
    // main blog post model - matches json structure from blog-posts.json
    // contains all blog properties inc comments collection for nested data
    public class BlogPost
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string HtmlContent { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}