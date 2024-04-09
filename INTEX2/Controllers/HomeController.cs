using INTEX2.Areas.Identity.Pages.Account;
using INTEX2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Diagnostics;

namespace INTEX2.Controllers
{
    /*
    A user has to login in order to access ANY pieces of the controller
    Using this will automaticall redirect a user to the login page to access any
    of these controller routes
     */
    //[Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Tools _tools;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(ILogger<HomeController> logger, Tools tools, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _tools = tools;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //Grab the user by the user's name
            var userClaim = HttpContext.User.Identity?.Name;
            var user = _userManager.FindByNameAsync(userClaim);

            if (userClaim == null)
            {
                ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                return View();
            }
            else
            {
                ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                ViewBag.UserName = user.Result?.FirstName;

                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Secrets() 
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ProductsPage()
        {

            //Grab the user by the user's name
            var userClaim = HttpContext.User.Identity?.Name;
            var user = _userManager.FindByNameAsync(userClaim);

            if (userClaim == null)
            {
                ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                return View();
            }
            else
            {
                ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                ViewBag.UserName = user.Result?.FirstName;

                return View();
            }
            
        }

        public IActionResult About()
        {

            //Grab the user by the user's name
            var userClaim = HttpContext.User.Identity?.Name;
            var user = _userManager.FindByNameAsync(userClaim);

            if (userClaim == null)
            {
                ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                return View();
            }
            else
            {
                ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                ViewBag.UserName = user.Result?.FirstName;

                return View();
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
