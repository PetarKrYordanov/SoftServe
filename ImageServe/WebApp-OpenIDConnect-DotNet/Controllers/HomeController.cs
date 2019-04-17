namespace ImageServe.Controllers
{

    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    using ImageServe.Services.Contracts;

    [Authorize]
    public class HomeController : Controller
    {
        private readonly AzureAdB2COptions AzureAdB2COptions;
        private readonly IUserService userService;
        private readonly IImageService imageService;

        public HomeController(IOptions<AzureAdB2COptions> azureAdB2COptions, IUserService userService, IImageService imageService)
        {
            AzureAdB2COptions = azureAdB2COptions.Value;
            this.userService = userService;
            this.imageService = imageService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            //TODO ask how should we check only once after sign in
            

            var pageSize = 9;
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
           

            var images =   imageService.Newest(pageNumber, pageSize);
            
            return View(images);
        }
        
        public IActionResult Error(string message)
        {
            ViewBag.Message = message;
            return View();
        }
        


    }
}
