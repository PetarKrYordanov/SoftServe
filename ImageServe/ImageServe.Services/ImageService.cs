  namespace ImageServe.Services
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    using BlobStorageTools.Contracts;
    using ImageServe.Data.Common;
    using ImageServe.Models;
    using ImageServe.Services.Contracts;
    using ImageServe.WebModels.BindingModels;
    using ImageServe.WebModels.Dtos;
    using ImageServe.WebModels.ViewModels;

    using AutoMapper;
    using PagedList.Core;

    public class ImageService : IImageService
    {
        private string[] SUPPORTED_FORMATS = new string[4] { ".jpeg", ".jpg", ".png", ".bmp" };

        private readonly IRepository<Image> images;
        private readonly IMapper mapper;
        private readonly ITagService tagService;
        private readonly IUserService userService;
        private readonly IBlobStorageService blobStorageService;
        private readonly IRepository<ImageLike> likes;

        public ImageService(
            IRepository<Image> images,
            IMapper mapper,
            ITagService tagService,
            IUserService userService,
            IBlobStorageService blobStorageService,
            IRepository<ImageLike> likes)
        {
            this.images = images;
            this.mapper = mapper;
            this.tagService = tagService;
            this.userService = userService;
            this.blobStorageService = blobStorageService;
            this.likes = likes;
        }

        public async Task AddAsync(ImageUploadBindingModel uploadBindingModel)
        {

            var file = await this.ToByteArrayAsync(uploadBindingModel.File);
            //var fileExtension = Path.GetExtension(uploadBindingModel.File.FileName);
            var fileExtension = this.GetExtension(uploadBindingModel.File.FileName);
            var fileName = this.GenerateName(uploadBindingModel.File.FileName, fileExtension);

            if (FileIsEmpty(file))
            {
                throw new FileNotFoundException();
            }

            if (!SUPPORTED_FORMATS.Contains(fileExtension))
            {
                throw new FormatException("Please select a .jpeg, .png or .bmp file");
            }

            await this.blobStorageService.UploadFromFileAsync(file, fileName, fileExtension);

            var userId = this.userService.GetCurrentId();

            var image = new Image()
            {
                Name = fileName,
                Description = uploadBindingModel.Description,
                UserId = userId,
                IsPublic = uploadBindingModel.IsPublic
            };

            await tagService.AddToImageAsync(image);
            await this.images.AddAsync(image);
            await this.images.SaveChangesAsync();
        }


        public ICollection<ImageDto> AllByUser(string userId)
        {
            var currentUserId = this.userService.GetCurrentId();

            var imagesByUser = this.images.All()
                .Where(i => i.UserId == userId)
                .Where(i =>
                    //same user
                    userId == currentUserId
                    //public
                    || i.IsPublic
                    //user's friend (this.userService.AreFriends(currentUserId, userId))
                    || i.User.UserFriends.Any(f => f.FriendId == currentUserId))
                .Select(this.mapper.Map<ImageDto>)
                .ToList();

            return imagesByUser;


            /*var images = this.images.All().Where(i => i.UserId == userId);
            var userFriendlist = images.FirstOrDefault()?.User.UserFriends;



            var currentUserId = this.userService.GetCurrentId();

            if (userId != currentUserId && userFriendlist?.Any(f=>f.FriendId == currentUserId) == false)
            {
                images = images.Where(i => i.IsPublic);
            }

            return images.Select(this.mapper.Map<ImageDto>).ToList();*/
            //return images.OrderByDescending(i=>i.DateUploaded).Select(this.mapper.Map<ImageDto>).ToList();

        }

        public async Task<T> GetImageAsync<T>(int imageId)
        {
            var image = await images.All().SingleOrDefaultAsync(i => i.Id == imageId);

            var currentUserId = this.userService.GetCurrentId();

            if (!image.IsPublic && image.UserId != currentUserId && !image.User.UserFriends.Any(f => f.FriendId == currentUserId))
            {
                throw new AccessViolationException("This image is private");
            }

            var imageModel = this.mapper.Map<T>(image);
            return imageModel;
        }




        public async Task EditAsync(ImageEditBindingModel imageEditBindingModel)
        {
            var currUserId = this.userService.GetCurrentId();
            var image = await GetImageAsync<Image>(imageEditBindingModel.Id);

            if (currUserId != image.UserId)
            {
                return;
            }

            image.Description = imageEditBindingModel.Description;
            image.IsPublic = imageEditBindingModel.IsPublic;

            await tagService.UpdateDescriptionTagsAsync(image);

            await this.images.SaveChangesAsync();

        }

        public async Task RemoveAsync(int imageId)
        {
            var currUserId = this.userService.GetCurrentId();
            var image = await GetImageAsync<Image>(imageId);

            if (currUserId != image.UserId)
            {
                return;
            }
            //todo: put all in transaction
            await blobStorageService.DeleteImageAsync(image.Name);
            await blobStorageService.DeleteImageAsync(image.Name + "-thumb.jpg");
            if (image.User.Avatar == image.Name)
            {
                image.User.Avatar = null;
            }
            
            images.Delete(image);
            await images.SaveChangesAsync();
        }

        public IPagedList<ImageViewModel> Newest(int page, int pageSize)
        {
            var currentUserId = this.userService.GetCurrentId();
            var query = this.images.All()
                .Where(i => i.IsPublic || i.UserId == currentUserId || i.User.UserFriends.Any(f => f.FriendId == currentUserId))
                .OrderByDescending(d => d.DateUploaded);
            
            var latestImages =    query          
                .Skip((page- 1) * pageSize).Take(pageSize)
                   .Select(mapper.Map<ImageViewModel>);


            var count = query.Count();

           var currentPage =  new StaticPagedList<ImageViewModel>(latestImages, page, pageSize, count);
            return currentPage;
        }

        public async Task<bool> ToggleLikeAsync(string userId, int imageId)
        {
            try
            {
                var like = likes.All()
                    .FirstOrDefault(l => l.ImageID == imageId && l.UserId == userId);

                if (like != null)
                {
                    likes.Delete(like);
                    return false;
                }
                else
                {
                    await likes.AddAsync(new ImageLike { UserId = userId, ImageID = imageId });
                    return true;
                }
            }
            finally
            {
                await likes.SaveChangesAsync();
            }
        }

        public int GetLikesCount(int imageId)
        {
            return likes.All().Count(l => l.ImageID == imageId);
        }

        public bool IsLiked(string userId, int imageId)
        {
            return likes.All().Any(l => l.ImageID == imageId && l.UserId == userId);
        }

        public async Task AddComment(CommentViewModel vm)
        {
            var image = await this.GetImageAsync<Image>(vm.ImageId);

            if (vm.MainCommentId == 0 )
            {
                
                image.MainComments.Add(new MainComment()
                {
                    Message = vm.Message,
                    Created = DateTime.Now,
                    UserName = this.userService.GetCurrentUserNames(),
                });
            }
            else
            {
                var mainComment = image.MainComments.FirstOrDefault(c => c.Id == vm.MainCommentId);
                  var comment = new SubComment()
                  {
                      MainCommentId = vm.MainCommentId,
                      Message = vm.Message,
                      Created = DateTime.Now,
                      UserName = this.userService.GetCurrentUserNames(),
                  };
                  mainComment.SubComments.Add(comment);
            }

            await this.images.SaveChangesAsync();

        }

        private bool FileIsEmpty(byte[] file)
        {
            return (file?.Length ?? 0) == 0;
        }


        private string GenerateName(string fileName, string extension)
        {
            //generate a random name for every uploaded file so names never match
            fileName = Path.GetFileNameWithoutExtension(fileName);
            return $"{fileName}-{Guid.NewGuid().ToString()}{extension}";
        }


        private async Task<byte[]> ToByteArrayAsync(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }


        private string GetExtension(string fileName)
        {
            return "." + fileName.Split(".").Last();
        }

       
    }
}
