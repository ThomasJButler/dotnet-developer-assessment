using System.Collections.Generic;

namespace DeveloperAssessment.Web.Models
{
    // wrapper model for json deserialisation - matches root structure of blog-posts.json
    // contains list of blogposts as json has blogPosts array at root level
    public class BlogData
    {
        public List<BlogPost> BlogPosts { get; set; }
    }
}