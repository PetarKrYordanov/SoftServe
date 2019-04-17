namespace ImageServe.Models
{
    public class ImageLike
    {
        public int ImageID { get; set; }
        public virtual Image Image { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
