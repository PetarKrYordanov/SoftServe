namespace ImageServe.Models
{
    public class SubComment : Comment
    {
        public int MainCommentId { get; set; }

        public virtual MainComment MainComment { get; set; }
    }
}
