namespace ImageServe.WebModels.ViewModels
{
using System.ComponentModel.DataAnnotations;

    public class CommentViewModel
    {
        [Required]
        public int ImageId { get; set; }

        [Required]
        public int MainCommentId { get; set; }

        [Required]
        public string Message { get; set; }

        
        
    }
}
