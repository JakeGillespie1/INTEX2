using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using INTEX2.Models;
using Microsoft.AspNetCore.Identity;
using INTEX2.Infrastructure;


namespace INTEX2.Pages
{
    public class CartModel : PageModel
    {

        public string ReturnUrlC { get; set; } = "/";

        private IINTEX2Repository _repo;
        private Tools _tools; // Assuming _tools is an instance of ITools
       

        public CartModel(IINTEX2Repository temp, Tools tools, Cart cartService)
        {
            _repo = temp;
            _tools = tools;
            Cart = cartService;
            
        }

        public Cart? Cart { get; set; }


        public void OnGet(string returnUrlC)
        {
            ReturnUrlC = returnUrlC ?? "/";
            SetViewDataTimeOfDay();
        }


        public IActionResult OnPost(int productId, string returnUrlC)
        {
            Product prod = _repo.Products.FirstOrDefault(x => x.ProductId == productId);

            if (prod != null)
            {  
                Cart.AddItem(prod, 1);
          
            }
            
            return RedirectToPage (new {returnUrlC});
            
        }

        public IActionResult OnPostRemove(int productId, string returnUrlC)
        {
            Cart.RemoveLine(Cart.Lines.First(x => x.Product.ProductId == productId).Product);
            return RedirectToPage (new {returnUrlC});

        }


        private void SetViewDataTimeOfDay()
        {
            var userClaim = HttpContext.User.Identity?.Name;


            if (userClaim == null)
            {
                ViewData["TimeOfDay"] = _tools.GetTimeOfDay();
            }
            else
            {
                ViewData["TimeOfDay"] = _tools.GetTimeOfDay();

            }
        }

    }
}
