using INTEX2.Areas.Identity.Pages.Account;
using INTEX2.Models;
using INTEX2.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Diagnostics;
using System.Drawing;

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
        private ILogger<HomeController> _logger;
        private Tools _tools;
        private SignInManager<AppUser> _signInManager;
        private UserManager<AppUser> _userManager;
        private readonly IINTEX2Repository _repo;

        public HomeController(ILogger<HomeController> logger, Tools tools, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IINTEX2Repository repo)
        {
            _logger = logger;
            _tools = tools;
            _signInManager = signInManager;
            _userManager = userManager;
            _repo = repo;
        }

        public IActionResult Index()
        {
            //Grab the user by the user's name
            var userClaim = HttpContext.User.Identity?.Name;
            var user = _userManager.FindByNameAsync(userClaim);

            var mostPurchasedProducts = _repo.GetMostPurchasedProducts();
            var viewBagListMP = new List<dynamic>();
            foreach (var product in mostPurchasedProducts)
            {
                dynamic dynamicProduct = product;
                viewBagListMP.Add(dynamicProduct.TopProdImgTag);
                viewBagListMP.Add(dynamicProduct.TopProdId);
                viewBagListMP.Add(dynamicProduct.TopProdName);
                viewBagListMP.Add(dynamicProduct.TopProdPrice);
            }
            ViewBag.MostPurchasedProducts = viewBagListMP;

            var mostHighlyRatedProducts = _repo.GetTopRatedProducts();
            var viewBagListTR = new List<dynamic>();
            foreach (var product in mostHighlyRatedProducts)
            {
                dynamic dynamicProduct = product;
                viewBagListTR.Add(dynamicProduct.TopProdImgTag);
                viewBagListTR.Add(dynamicProduct.TopProdId);
                viewBagListTR.Add(dynamicProduct.TopProdName);
                viewBagListTR.Add(dynamicProduct.TopProdPrice);
            }
            ViewBag.MostHighlyRatedProducts = viewBagListTR;

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
        public IActionResult Privacy()
        {
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
        public IActionResult ProductsPage(int page = 1, int? pageSize = null, string color = null, string category = null)
        {
            if (!pageSize.HasValue)
            {
                // Set default page size
                pageSize = 10;
            }
            var distinctColors = _repo.Products.Select(p => p.PrimaryColor).Distinct().ToList();
            var distinctCategories = _repo.Categories.Select(c => c.CategoryName).Distinct().ToList();

            ViewBag.ColorOptions = GetSelectOptions(distinctColors, color);
            ViewBag.CategoryOptions = GetSelectOptions(distinctCategories, category);


            // Calculate the number of items to skip
            int skip = (page - 1) * pageSize.Value;

            var productsQuery = _repo.Products.AsQueryable();

            // Apply color filter if specified
            if (!string.IsNullOrEmpty(color))
            {
                productsQuery = productsQuery.Where(p => p.PrimaryColor == color);
            }

            // Apply category filter if specified
            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Categories.Any(c => c.CategoryName == category));
            }


            // Get total count of products
            int totalProductsCount = productsQuery.Count();


            // Calculate total number of pages
            int totalPages = (int)Math.Ceiling((double)totalProductsCount / pageSize.Value);

            // Get products for the current page
            var productData = productsQuery
                .OrderBy(x => x.ProductId)
                .Skip(skip)
                .Take(pageSize.Value)
                .ToList();

            ViewBag.TimeOfDay = _tools.GetTimeOfDay();
            ViewBag.TotalProductsCount = totalProductsCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize.Value;
            ViewBag.TotalPages = totalPages;
            ViewBag.SelectedColor = color;
            ViewBag.SelectedCategory = category;
            //Grab the user by the user's name
            var userClaim = HttpContext.User.Identity?.Name;
            var user = _userManager.FindByNameAsync(userClaim);

            if (userClaim != null)
            {
                ViewBag.UserName = user.Result?.FirstName;
            }

            return View(productData);
        
        }
        private List<SelectListItem> GetSelectOptions(List<string> values, string selectedValue)
        {
            var options = new List<SelectListItem>();

            options.Add(new SelectListItem { Text = "All", Value = "" });

            foreach (var value in values)
            {
                options.Add(new SelectListItem { Text = value, Value = value, Selected = value == selectedValue });
            }

            return options;
        }

        public IActionResult ProductDetail(int id)
        {
            // Retrieve product details from database
            var product = _repo.GetProductById(id);

            // Retrieve recommendations for the product
            var recommendations = _repo.ProductBasedRecommendations
                .FirstOrDefault(pr => pr.ProductId == id);



            string rec1 = recommendations.Rec1;
            string rec2 = recommendations.Rec2;
            string rec3 = recommendations.Rec3;

            var recommendationsList = _repo.GetProductRecs(rec1, rec2, rec3);

            var userClaim = HttpContext.User.Identity?.Name;
            var user = _userManager.FindByNameAsync(userClaim);

            if (userClaim == null)
            {
                ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                ViewBag.Recommendations = recommendationsList; // Pass recommendations to the view
                return View("ProductDetail", product);
            }
            else
            {
                ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                ViewBag.UserName = user.Result?.FirstName;
                ViewBag.Recommendations = recommendationsList; // Pass recommendations to the view
                return View("ProductDetail", product);
            }
            // Pass product model to ProductDetail.cshtml view
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
