// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using PartsUnlimited.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace PartsUnlimited.api.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IProductsRepository productsRepository;

        public ProductsController(IProductsRepository productsRepository)
        {
            this.productsRepository = productsRepository;
        }

        [HttpGet]
        public IEnumerable<Product> Get(bool sale = false)
        {
            if (!sale)
            {
                return this.productsRepository.Set<Product>().AsEnumerable();
            }

            return this.productsRepository.Find(p => p.Price != p.SalePrice);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var product = this.productsRepository.GetById(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return new ObjectResult(product);
        }
    }
}
