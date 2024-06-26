﻿using INTEX2.Models;
using Microsoft.AspNetCore.Mvc;

namespace INTEX2.Components
{
    public class CartSummaryViewComponent :ViewComponent
    {
        private Cart cart;
        public CartSummaryViewComponent(Cart cartService)
        {
            cart = cartService;
        }
        public IViewComponentResult Invoke()
        {
            return View(cart);
        }
    }
}
