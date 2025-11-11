using System.ComponentModel.DataAnnotations;

namespace DeveloperAssessment.Web.Models.ViewModels
{
    // form view model for comment submission - validates user input via data annotations
    // keeps validation logic centralised & enables client/server-side validation
    public class AddCommentViewModel
    {
        public int BlogPostId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Comment is required")]
        public string Message { get; set; }
    }
}