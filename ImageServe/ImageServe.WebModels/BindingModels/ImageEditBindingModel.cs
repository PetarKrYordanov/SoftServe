namespace ImageServe.WebModels.BindingModels
{
    using System;
    using System.Collections.Generic;

    using ImageServe.WebModels.Dtos;

    public class ImageEditBindingModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        public DateTime DateUploaded { get; set; }

        public UserDto User { get; set; }

        public string GetImageUrl() => "https://netacademy.blob.core.windows.net/images/" + this.Name;

        public ICollection<TagDto> Tags { get; set; }

        public string CurrentUser { get; set; }

    }
}
