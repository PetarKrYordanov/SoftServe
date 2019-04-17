namespace ImageServe.Models
{
    using System.Collections.Generic;

    public class MainComment: Comment
    {
        public int ImageId { get; set; }
        public virtual Image Image { get; set; }
        public virtual List<SubComment> SubComments { get; set; }
    }
}
