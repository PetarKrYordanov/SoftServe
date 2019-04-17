namespace ImageServe.WebModels.ViewModels
{
    using System;
    using System.Collections.Generic;

    using ImageServe.Models;
    using ImageServe.WebModels.Dtos;

    public class ImageViewModel
    {

        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public DateTime DateUploaded { get; set; }

        public UserDto User { get; set; }

        public ICollection<MainComment> MainComments { get; set; }

        public ICollection<TagDto> Tags { get; set; }

        public bool IsPublic { get; set; }

        public string GetImageUrl() => "https://netacademy.blob.core.windows.net/images/" + this.Name;

        public string GetThumbUrl() => "https://netacademy.blob.core.windows.net/thumbs/" + this.Name + "-thumb.jpg";

        public bool IsLiked { get; set; }

        public int LikesCount { get; set; }

        public string CurrentUser { get; set; }


       
    }
}
