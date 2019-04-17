namespace ImageServe.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using ImageServe.Services.Contracts;
    using ImageServe.WebModels.Dtos;
    using ImageServe.WebModels.ViewModels;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using PagedList.Core;

    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService userService;
        private readonly IImageService imageService;
        private readonly string currentUserId;

        public ProfileController(IUserService userService, IImageService imageService)
        {
            this.userService = userService;
            this.imageService = imageService;
            this.currentUserId = this.userService.GetCurrentId();
        }
        
        public async Task<IActionResult> Details(string id, int? page)
        {
                if (id is null)
                {
                    id = currentUserId;
                }
                
            var pageSize = 4;
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;

            var user = await this.userService.SingleOrDefaultAsync<UserViewModel>(u => u.Id == id);
            user.Images = this.imageService.AllByUser(id).AsQueryable().ToPagedList<ImageDto>(pageNumber, pageSize);
            
            return View(user);
        }

        public IActionResult Edit()
        {
            var currentUser = userService.GetCurrent<UserEditViewModel>();
            return View(currentUser);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string details)
        {
            await userService.SaveDescriptionAsync(details);
            return RedirectToAction("Details");
        }
        
        public async Task<IActionResult> SetAvatar(string name)
        {
            await userService.SetAvatarAsync(name);
            return RedirectToAction("Details");
        }
        
        public  IActionResult All()
        {
            var allUserNames = this.userService.AllFullNames();
            
            return Ok(allUserNames);
        }

        public async Task<IActionResult> Friendlist()
        {
            var user =  this.userService.GetCurrent<UserFriendlistViewModel>();

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Friendlist(UserFriendlistViewModel userPrivacyModel)
        {
            try
            {
                await this.userService.AddFriendAsync(userPrivacyModel.AddFriend);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("AddFriend", ex.Message);
            }
            

            var user = this.userService.GetCurrent<UserFriendlistViewModel>();

            return View(user);

        }

        public async Task<IActionResult> RemoveFriend(string friendId)
        {
            await this.userService.RemoveFriendAsync(friendId);

            return RedirectToAction("Friendlist");
        }
    }
}