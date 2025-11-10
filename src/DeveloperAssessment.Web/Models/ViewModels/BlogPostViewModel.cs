using System;
using System.Collections.Generic;

namespace DeveloperAssessment.Web.Models.ViewModels
{
    // view model for blog display - separates presentation logic from domain model
    // includes formatteddate helper to avoid date formatting in view
    public class BlogPostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Image { get; set; }
        public string HtmlContent { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();

        // Helper for display
        public string FormattedDate => Date.ToString("MMMM d, yyyy");
    }
}