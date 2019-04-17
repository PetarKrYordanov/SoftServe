namespace ImageServe.Services.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ImageServe.WebModels.BindingModels;
    using ImageServe.WebModels.Dtos;
    using ImageServe.WebModels.ViewModels;

    using PagedList.Core;

    public interface IImageService
    {
        Task AddAsync(ImageUploadBindingModel uploadBindingModel);

        ICollection<ImageDto> AllByUser(string userId);

        Task<T> GetImageAsync<T>(int imageId);

        IPagedList<ImageViewModel> Newest(int pageNumber, int pageSize);

        Task EditAsync(ImageEditBindingModel image);

        Task<bool> ToggleLikeAsync(string userId, int imageId);

        int GetLikesCount(int imageId);
         
        bool IsLiked(string userId, int imageId);

        Task RemoveAsync(int imageId);

         Task AddComment(CommentViewModel vm);

        //bool Exists(int id);
        //
        //TModel ById<TModel>(int id);

    }
}
