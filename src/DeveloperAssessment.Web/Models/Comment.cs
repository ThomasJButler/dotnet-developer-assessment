using System;
using System.Collections.Generic;

namespace DeveloperAssessment.Web.Models
{
    // comment model - represents user comments & replies on blog posts
    // recursive structure via replies list allows nested comment threads
    public class Comment
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string EmailAddress { get; set; }
        public string Message { get; set; }
        public List<Comment> Replies { get; set; } = new List<Comment>();
    }
}