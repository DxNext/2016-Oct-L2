// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace PartsUnlimited.Models
{
    public class Product
    {
        public string SkuNumber { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get
            {
                return this.ProductId.ToString();
            }

            set { }
        }

        public int ProductId { get; set; }

        public int RecommendationId { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public decimal SalePrice { get; set; }
        public decimal CostPrice { get; set; }

        public string ProductArtUrl { get; set; }

        public virtual Category Category { get; set; }

        public DateTime Created { get; set; }

        public string Description { get; set; }

        public string ProductDetails { get; set; }

        public int Inventory { get; set; }

        public int LeadTime { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> ProductDetailList
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ProductDetails))
                {
                    return new Dictionary<string, string>();
                }
                try
                {
                    var obj = JToken.Parse(ProductDetails);
                }
                catch (Exception)
                {
                    throw new FormatException("Product Details only accepts json format.");
                }
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(ProductDetails);
            }
        }

        /// <summary>
        /// TODO: Temporary hack to populate the orderdetails until EF does this automatically. 
        /// </summary>
        public Product()
        {
            Created = DateTime.UtcNow;
        }
    }
}
