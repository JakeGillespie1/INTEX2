using INTEX2.Areas.Identity.Pages.Account;
using INTEX2.Models;
using INTEX2.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Diagnostics;
using Microsoft.ML;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Azure.Security.KeyVault.Certificates;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;


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
        private readonly InferenceSession _inferenceSession;

        public HomeController(ILogger<HomeController> logger, Tools tools, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IINTEX2Repository repo)
        {
            _logger = logger;
            _tools = tools;
            _signInManager = signInManager;
            _userManager = userManager;
            _repo = repo;


            try
            {
                _inferenceSession = new InferenceSession("C:\\Users\\jakeg\\source\\repos\\INTEX2\\INTEX2\\gradient_boost_model.onnx");
                _logger.LogInformation("ONNX model loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading the ONNX model: {ex.Message}");
            }


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
            else if (user.Result?.FirstName != "Admin")
            {
                ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                ViewBag.UserName = user.Result?.FirstName;

                return View();
            }
            else if (user.Result?.FirstName == "Admin" && user.Result?.LastName == "Admin" && user.Result?.Email == "admin@admin.com")
            {
                ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                ViewBag.UserName = user.Result?.FirstName;

                return RedirectToAction("AdminIndex");
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminIndex()
        {
            var userClaim = HttpContext.User.Identity?.Name;
            var user = _userManager.FindByNameAsync(userClaim);
            ViewBag.TimeOfDay = _tools.GetTimeOfDay();
            ViewBag.UserName = user.Result?.FirstName;
            return View();
        }

        [HttpPost]
        public IActionResult Cart(string CartTotal)
        {
            var userClaim = HttpContext.User.Identity?.Name;
            var user = _userManager.FindByNameAsync(userClaim);

            if (userClaim == null) 
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            ViewBag.TimeOfDay = _tools.GetTimeOfDay();
            ViewBag.UserName = user.Result?.FirstName;

            ViewBag.CartTotal = CartTotal;
            return View("Checkout");
        }

        [HttpPost]
        public IActionResult CheckoutCart()
        {
            var userClaim = HttpContext.User.Identity?.Name;
            var user = _userManager.FindByNameAsync(userClaim);

            if (userClaim == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            ViewBag.TimeOfDay = _tools.GetTimeOfDay();
            ViewBag.UserName = user.Result?.FirstName;
            return View();
        }

        [HttpPost]
        public IActionResult FraudPrediction(Order order)
        {
            //Initialize variables to put into the prediction model list
            int time = 0;
            int amount = 0;
            int monday = 0;
            int saturday = 0;
            int sunday = 0;
            int thursday = 0;
            int tuesday = 0;
            int wednesday = 0;
            int pin = 0;
            int tap = 0;
            int onilne = 0;
            int POS = 0; 
            int india = 0;
            int russia = 0;
            int usa = 0;
            int uk = 0;
            int shipIndia = 0;
            int shipRuss = 0;
            int shipUS = 0;
            int shipUK = 0;
            int bankHSBC = 0;
            int bankHalifax = 0;
            int bankLloyds = 0;
            int bankMetro = 0;
            int bankMonzo = 0;
            int bankRBS = 0;
            int visa = 0;

            //Add order (minus prediction) to db
            _repo.AddOrder(order);

            //Dictionary mapping the numeric prediction to potential fraud...
            var class_type_dict = new Dictionary<int,string>()
            {
                {0 , "Not Fraudulent" },
                {1, "Potentially Fraudulent Purchase" }
            };

            //Initialize the input list
            var input = new List<float> { time, amount, monday, saturday, sunday, thursday, tuesday, wednesday,
                pin, tap, onilne, POS, india, russia, usa, uk, shipIndia, shipRuss, shipUS, shipUK,
                bankHSBC, bankHalifax, bankLloyds, bankMetro, bankMonzo, bankRBS, visa };

            //Scan through the order and place the inputs into the model...
            //Insert amount and time of day
            input[0] = order.Time;
            input[1] = (float)order.Amount;

            //Insert Day of week
            if (order.DayOfWeek == "Mon")
            {
                input[2] = 1;
            }
            else if (order.DayOfWeek == "Sat")
            {
                input[3] = 1;
            }
            else if (order.DayOfWeek == "Sun")
            {
                input[4] = 1;
            }
            else if (order.DayOfWeek == "Thu")
            {
                input[5] = 1;
            }
            else if (order.DayOfWeek == "Tue")
            {
                input[6] = 1;
            }
            else if (order.DayOfWeek == "Wed")
            {
                input[7] = 1;
            }

            //Insert Payment Type
            if (order.EntryMode == "PIN")
            {
                input[8] = 1;
            }
            else if (order.EntryMode == "Tap")
            {
                input[9] = 1;
            }

            //Insert type of transaction
            if (order.TypeOfTransaction == "Online")
            {
                input[10] = 1;
            }
            else if (order.TypeOfTransaction == "POS")
            {
                input[11] = 1;
            }

            //insert country of origin
            if (order.CountryOfTransaction == "India")
            {
                input[12] = 1;
            }
            else if (order.CountryOfTransaction == "Russia")
            {
                input[13] = 1;
            }
            else if (order.CountryOfTransaction == "USA")
            {
                input[14] = 1;
            }
            else if(order.CountryOfTransaction == "United Kingdom")
            {
                input[15] = 1;
            }

            //insert shipping address
            if (order.ShippingAddress == "India")
            {
                input[16] = 1;
            }
            else if (order.ShippingAddress == "Russia")
            {
                input[17] = 1;
            }
            else if (order.ShippingAddress == "USA")
            {
                input[18] = 1;
            }
            else if (order.ShippingAddress == "United Kingdom")
            {
                input[19] = 1;
            }

            //insert bank
            if (order.Bank == "HSBC")
            {
                input[20] = 1;
            }
            else if (order.Bank =="Halifax")
            {
                input[21] = 1;
            }
            else if (order.Bank == "Lloyds")
            {
                input[22] = 1;
            }
            else if (order.Bank == "Metro")
            {
                input[23] = 1;
            }
            else if (order.Bank == "Monzo")
            {
                input[24] = 1;
            }
            else if (order.Bank == "RBS")
            {
                input[25] = 1;
            }

            //insert type of card info
            if (order.TypeOfCard == "Visa")
            {
                input[26] = 1;
            }

            try
            {
                var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });
                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
                };

                using (var results = _inferenceSession.Run(inputs)) //make prediction from the inputs from the payment info form (smae as model.predict)
                {
                    var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
                    if (prediction != null && prediction.Length > 0)
                    {
                        //Use the prediction to get the fraud prediction from the directory (1 or 0)
                        var isFraud = class_type_dict.GetValueOrDefault((int)prediction[0], "Unknown");
                        ViewBag.Prediction = isFraud;
                        ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                        if (isFraud == "Potentially Fraudulent Purchase")
                        {
                            //Save new prediction to database (if it is fraudulent) as well!!
                            _repo.AddFraudPredictionToOrder(order.OrderId);
                        }

                        return View();
                    }
                    else
                    {
                        ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                        return View();
                    }
                }  
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Error loading the ONNX model: {ex.Message}");
                return RedirectToAction("Index");
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
        public IActionResult ProductsPage(int page = 1, int? pageSize = null)
        {
            if (!pageSize.HasValue)
            {
                // Set default page size
                pageSize = 10;
            }

            // Calculate the number of items to skip
            int skip = (page - 1) * pageSize.Value;

            // Get total count of products
            int totalProductsCount = _repo.Products.Count();

            // Calculate total number of pages
            int totalPages = (int)Math.Ceiling((double)totalProductsCount / pageSize.Value);

            // Get products for the current page
            var productData = _repo.Products
                .OrderBy(x => x.ProductId)
                .Skip(skip)
                .Take(pageSize.Value)
                .ToList();

            //Grab the user by the user's name
            var userClaim = HttpContext.User.Identity?.Name;
            var user = _userManager.FindByNameAsync(userClaim);

            if (userClaim == null)
            {
                ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                ViewBag.TotalProductsCount = totalProductsCount;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize.Value;
                ViewBag.TotalPages = totalPages;
                return View(productData);
            }
            else
            {
                ViewBag.TimeOfDay = _tools.GetTimeOfDay();
                ViewBag.UserName = user.Result?.FirstName;
                ViewBag.TotalProductsCount = totalProductsCount;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize.Value;
                ViewBag.TotalPages = totalPages;
                return View(productData);
            }
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
