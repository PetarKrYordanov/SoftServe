namespace ImageServe.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Image
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Image()
        {         
           this.Tags = new List<ImageTag>();
           this.MainComments = new List<MainComment>();
        }

        public string Name { get; set; }

        //[Required]
        //public string Url { get; set; } 

        public string Description { get; set; }

        public DateTime DateUploaded { get; set; }

        public bool IsPublic { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<ImageTag> Tags { get; set; }

        public virtual ICollection<MainComment> MainComments { get; set; }

        public virtual ICollection<ImageLike> Likes { get; set; }

        public string GetImageUrl() => "https://netacademy.blob.core.windows.net/images/" + this.Name;

        public string GetThumbUrl() => "https://netacademy.blob.core.windows.net/thumbs/" + this.Name + "-thumb.jpg";
    }
}

        
