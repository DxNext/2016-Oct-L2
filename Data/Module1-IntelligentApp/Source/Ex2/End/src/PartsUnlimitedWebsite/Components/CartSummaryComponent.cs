// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using PartsUnlimited.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PartsUnlimited.Components
{
    [ViewComponent(Name = "CartSummary")]
    public class CartSummaryComponent : ViewComponent
    {
        private readonly IPartsUnlimitedContext _db;
        private readonly IProductsRepository productsRepository;

        public CartSummaryComponent(IPartsUnlimitedContext context, IProductsRepository productsRepository)
        {
            _db = context;
            this.productsRepository = productsRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cartItems = await GetCartItems();

            ViewBag.CartCount = cartItems.Select(x => x.Count).Sum();
            ViewBag.CartSummary = string.Join("\n", cartItems.Distinct());

            return View();
        }

        private Task<IOrderedEnumerable<CartSummeryComponentModel>> GetCartItems()
        {
            var cart = ShoppingCart.GetCart(_db, this.productsRepository, HttpContext);

            var cartItems = cart.GetCartItems()
                .Select(a => new CartSummeryComponentModel { Title = a.Product.Title, Count = a.Count })
                .OrderBy(x => x.Title);

            return Task.FromResult(cartItems);
        }
    }

    public class CartSummeryComponentModel
    {
        public int Count { get; internal set; }
        public string Title { get; internal set; }
    }
}