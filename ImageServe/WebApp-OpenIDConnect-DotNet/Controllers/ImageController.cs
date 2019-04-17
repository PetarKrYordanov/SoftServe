namespace ImageServe.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using ImageServe.Services.Contracts;
    using ImageServe.WebModels.BindingModels;
    using ImageServe.WebModels.ViewModels;



    [Authorize]
    public class ImageController : Controller
    {
        private readonly IImageService imageService;
        private readonly IUserService userService;

        public ImageController(IImageService imageService, IUserService userService)
        {
            this.imageService = imageService;
            this.userService = userService;
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ImageUploadBindingModel uploadBindingModel)
        {
            await this.imageService.AddAsync(uploadBindingModel);
            return RedirectToAction("Details","Profile");
        }
        
        
        public async Task<IActionResult> Details(int id)
        {
            ImageViewModel image;

            try
            {
                image = await this.imageService.GetImageAsync<ImageViewModel>(id);
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Home");
            }
        
            var currentUserId = userService.GetCurrentId();
            image.LikesCount = imageService.GetLikesCount(id);
            image.IsLiked = imageService.IsLiked(currentUserId, id);
            image.CurrentUser = currentUserId;
            var imageAuthor = userService.GetUserById(image.User.Id);
            
            if (image == null)
            {
                return RedirectToAction("Index", "Home");
            }
     
            return View(image);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var image = await imageService.GetImageAsync<ImageEditBindingModel>(id);
            if (image == null)
            {
                return RedirectToAction("Image", "Details");
            }

            var currentUserId = userService.GetCurrentId();
            image.CurrentUser = currentUserId;

            return View(image);
        }
        
        public async Task<IActionResult> Remove(int id)
        {
            await imageService.RemoveAsync(id);
            return RedirectToAction("Details", "Profile");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ImageEditBindingModel image)
        {
            await imageService.EditAsync(image);
            //fix redirect
            return Redirect("/Image/Details/" + image.Id);
         
        }

        public async Task<IActionResult> ToggleLike(string userId, int imageId)
        {
            var liked = await imageService.ToggleLikeAsync(userId, imageId);
            var likes = imageService.GetLikesCount(imageId);
            return Json(new { liked, likes });
        }

        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details",new {id = vm.ImageId});
            }

            await this.imageService.AddComment(vm);

            return RedirectToAction("Details", new { id = vm.ImageId });
        }


    }
}