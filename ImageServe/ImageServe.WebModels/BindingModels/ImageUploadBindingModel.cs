namespace ImageServe.WebModels.BindingModels
{
    using Microsoft.AspNetCore.Http;
    
    public class ImageUploadBindingModel
    {
        public IFormFile File { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        //TODO
        //public bool SetAsAvatar { get; set; }
    }
}
