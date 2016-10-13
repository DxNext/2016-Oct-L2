// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.Mvc;
using PartsUnlimited.Models;
using PartsUnlimited.Recommendations;
using PartsUnlimited.WebsiteConfiguration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PartsUnlimited.Controllers
{
    public class RecommendationsController : Controller
    {
        private readonly IRecommendationEngine _recommendation;
        private readonly IWebsiteOptions _option;
        private readonly IProductsRepository productsRepository;

        public RecommendationsController(IProductsRepository productsRepository, IRecommendationEngine recommendationEngine, IWebsiteOptions websiteOptions)
        {
            _recommendation = recommendationEngine;
            _option = websiteOptions;
            this.productsRepository = productsRepository;
        }

        public async Task<IActionResult> GetRecommendations(string recommendationId)
        {
            if (!_option.ShowRecommendations)
            {
                return new EmptyResult();
            }

            var recommendedProductIds = await _recommendation.GetRecommendationsAsync(recommendationId);

            var products = recommendedProductIds
                .Select(item => this.productsRepository.Find(c => c.RecommendationId == Convert.ToInt32(item)).FirstOrDefault())
                .ToList();

            var recommendedProducts = products
                .Where(p => p != null && p.RecommendationId != Convert.ToInt32(recommendationId))
                .ToList();

            return PartialView("_Recommendations", recommendedProducts);
        }
    }
}