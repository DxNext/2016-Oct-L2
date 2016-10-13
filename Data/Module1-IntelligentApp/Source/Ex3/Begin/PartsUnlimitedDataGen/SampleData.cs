// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;

namespace PartsUnlimited.Models
{
    public static class SampleData
    {
        public static IEnumerable<Category> GetCategories()
        {
            yield return new Category { Name = "Brakes", Description = "Brakes description", ImageUrl = "product_brakes_disc.jpg" };
            yield return new Category { Name = "Lighting", Description = "Lighting description", ImageUrl = "product_lighting_headlight.jpg" };
            yield return new Category { Name = "Wheels & Tires", Description = "Wheels & Tires description", ImageUrl = "product_wheel_rim.jpg" };
            yield return new Category { Name = "Batteries", Description = "Batteries description", ImageUrl = "product_batteries_basic-battery.jpg" };
            yield return new Category { Name = "Oil", Description = "Oil description", ImageUrl = "product_oil_premium-oil.jpg" };
        }

        public static IEnumerable<Product> GetProducts(IEnumerable<Category> categories)
        {
            var categoriesMap = categories.ToDictionary(c => c.Name, c => c);

            yield return new Product
            {
                SkuNumber = "LIG-0001",
                Title = "Halogen Headlights (2 Pack)",
                Category = categoriesMap["Lighting"],
                CategoryId = categoriesMap["Lighting"].CategoryId,
                CategoryName = categoriesMap["Lighting"].Name,
                Price = 38.99M,
                SalePrice = 38.99M,
                CostPrice = 16.99M,
                ProductArtUrl = "product_lighting_headlight.jpg",
                ProductDetails = "{ \"Light Source\" : \"Halogen\", \"Assembly Required\": \"Yes\", \"Color\" : \"Clear\", \"Interior\" : \"Chrome\", \"Beam\": \"low and high\", \"Wiring harness included\" : \"Yes\", \"Bulbs Included\" : \"No\",  \"Includes Parking Signal\" : \"Yes\"}",
                Description = "Our Halogen Headlights are made to fit majority of vehicles with our  universal fitting mold. Product requires some assembly.",
                Inventory = 10,
                LeadTime = 0,
                RecommendationId = 1,
                ProductId = 1
            };

            yield return new Product
            {
                SkuNumber = "LIG-0002",
                Title = "Bugeye Headlights (2 Pack)",
                Category = categoriesMap["Lighting"],
                CategoryId = categoriesMap["Lighting"].CategoryId,
                CategoryName = categoriesMap["Lighting"].Name,
                Price = 48.99M,
                SalePrice = 48.99M,
                CostPrice = 21.00M,
                ProductArtUrl = "product_lighting_bugeye-headlight.jpg",
                ProductDetails = "{ \"Light Source\" : \"Halogen\", \"Assembly Required\": \"Yes\", \"Color\" : \"Clear\", \"Interior\" : \"Chrome\", \"Beam\": \"low and high\", \"Wiring harness included\" : \"No\", \"Bulbs Included\" : \"Yes\",  \"Includes Parking Signal\" : \"Yes\"}",
                Description = "Our Bugeye Headlights use Halogen light bulbs are made to fit into a standard bugeye slot. Product requires some assembly and includes light bulbs.",
                Inventory = 7,
                LeadTime = 0,
                RecommendationId = 2,
                ProductId = 2
            };

            yield return new Product
            {
                SkuNumber = "LIG-0003",
                Title = "Turn Signal Light Bulb",
                Category = categoriesMap["Lighting"],
                CategoryId = categoriesMap["Lighting"].CategoryId,
                CategoryName = categoriesMap["Lighting"].Name,
                Price = 6.49M,
                SalePrice = 6.49M,
                CostPrice = 3.99M,
                ProductArtUrl = "product_lighting_lightbulb.jpg",
                ProductDetails = "{ \"Color\" : \"Clear\", \"Fit\" : \"Universal\", \"Wattage\" : \"30 Watts\", \"Includes Socket\" : \"Yes\"}",
                Description = " Clear bulb that with a universal fitting for all headlights/taillights.  Simple Installation, low wattage and a clear light for optimal visibility and efficiency.",
                Inventory = 18,
                LeadTime = 0,
                RecommendationId = 3,
                ProductId = 3
            };

            yield return new Product
            {
                SkuNumber = "WHE-0001",
                Title = "Matte Finish Rim",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                CategoryName = categoriesMap["Wheels & Tires"].Name,
                Price = 75.99M,
                SalePrice = 75.99M,
                CostPrice = 39.00M,
                ProductArtUrl = "product_wheel_rim.jpg",
                ProductDetails = "{ \"Material\" : \"Aluminum alloy\",  \"Design\" : \"Spoke\", \"Spokes\" : \"9\",  \"Number of Lugs\" : \"4\", \"Wheel Diameter\" : \"17 in.\", \"Color\" : \"Black\", \"Finish\" : \"Matte\" } ",
                Description = "A Parts Unlimited favorite, the Matte Finish Rim is affordable low profile style. Fits all low profile tires.",
                Inventory = 4,
                LeadTime = 0,
                RecommendationId = 4,
                ProductId = 4
            };

            yield return new Product
            {
                SkuNumber = "WHE-0002",
                Title = "Blue Performance Alloy Rim",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                CategoryName = categoriesMap["Wheels & Tires"].Name,
                Price = 88.99M,
                SalePrice = 88.99M,
                CostPrice = 34.50M,
                ProductArtUrl = "product_wheel_rim-blue.jpg",
                ProductDetails = "{ \"Material\" : \"Aluminum alloy\",  \"Design\" : \"Spoke\", \"Spokes\" : \"5\",  \"Number of Lugs\" : \"4\", \"Wheel Diameter\" : \"18 in.\", \"Color\" : \"Blue\", \"Finish\" : \"Glossy\" } ",
                Description = "Stand out from the crowd with a set of aftermarket blue rims to make you vehicle turn heads and at a price that will do the same.",
                Inventory = 8,
                LeadTime = 0,
                RecommendationId = 5,
                ProductId = 5
            };

            yield return new Product
            {
                SkuNumber = "WHE-0003",
                Title = "High Performance Rim",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                CategoryName = categoriesMap["Wheels & Tires"].Name,
                Price = 99.99M,
                SalePrice = 99.49M,
                CostPrice = 74.60M,
                ProductArtUrl = "product_wheel_rim-red.jpg",
                ProductDetails = "{ \"Material\" : \"Aluminum alloy\",  \"Design\" : \"Spoke\", \"Spokes\" : \"12\",  \"Number of Lugs\" : \"5\", \"Wheel Diameter\" : \"18 in.\", \"Color\" : \"Red\", \"Finish\" : \"Matte\" } ",
                Description = "Light Weight Rims with a twin cross spoke design for stability and reliable performance.",
                Inventory = 3,
                LeadTime = 0,
                RecommendationId = 6,
                ProductId = 6
            };

            yield return new Product
            {
                SkuNumber = "WHE-0004",
                Title = "Wheel Tire Combo",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                Price = 72.49M,
                SalePrice = 72.49M,
                CostPrice = 32.30M,
                ProductArtUrl = "product_wheel_tyre-wheel-combo.jpg",
                ProductDetails = "{ \"Material\" : \"Steel\",  \"Design\" : \"Spoke\", \"Spokes\" : \"8\",  \"Number of Lugs\" : \"4\", \"Wheel Diameter\" : \"19 in.\", \"Color\" : \"Gray\", \"Finish\" : \"Standard\", \"Pre-Assembled\" : \"Yes\" } ",
                Description = "For the endurance driver, take advantage of our best wearing tire yet. Composite rubber and a heavy duty steel rim.",
                Inventory = 0,
                LeadTime = 4,
                RecommendationId = 7,
                ProductId = 7
            };

            yield return new Product
            {
                SkuNumber = "WHE-0005",
                Title = "Chrome Rim Tire Combo",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                CategoryName = categoriesMap["Wheels & Tires"].Name,
                Price = 129.99M,
                SalePrice = 129.99M,
                CostPrice = 47.10M,
                ProductArtUrl = "product_wheel_tyre-rim-chrome-combo.jpg",
                ProductDetails = "{ \"Material\" : \"Aluminum alloy\",  \"Design\" : \"Spoke\", \"Spokes\" : \"10\",  \"Number of Lugs\" : \"5\", \"Wheel Diameter\" : \"17 in.\", \"Color\" : \"Silver\", \"Finish\" : \"Chrome\", \"Pre-Assembled\" : \"Yes\" } ",
                Description = "Save time and money with our ever popular wheel and tire combo. Pre-assembled and ready to go.",
                Inventory = 1,
                LeadTime = 0,
                RecommendationId = 8,
                ProductId = 8
            };

            yield return new Product
            {
                SkuNumber = "WHE-0006",
                Title = "Wheel Tire Combo (4 Pack)",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                CategoryName = categoriesMap["Wheels & Tires"].Name,
                Price = 219.99M,
                SalePrice = 219.99M,
                CostPrice = 89.40M,
                ProductArtUrl = "product_wheel_tyre-wheel-combo-pack.jpg",
                ProductDetails = "{ \"Material\" : \"Steel\",  \"Design\" : \"Spoke\", \"Spokes\" : \"8\",  \"Number of Lugs\" : \"5\", \"Wheel Diameter\" : \"19 in.\", \"Color\" : \"Gray\", \"Finish\" : \"Standard\", \"Pre-Assembled\" : \"Yes\" } ",
                Description = "Having trouble in the wet? Then try our special patent tire on a heavy duty steel rim. These wheels perform excellent in all conditions but were designed specifically for wet weather.",
                Inventory = 3,
                LeadTime = 0,
                RecommendationId = 9,
                ProductId = 9
            };

            yield return new Product
            {
                SkuNumber = "BRA-0001",
                Title = "Disk and Pad Combo",
                Category = categoriesMap["Wheels & Tires"],
                CategoryId = categoriesMap["Wheels & Tires"].CategoryId,
                CategoryName = categoriesMap["Wheels & Tires"].Name,
                Price = 25.99M,
                SalePrice = 25.99M,
                CostPrice = 16.40M,
                ProductArtUrl = "product_brakes_disk-pad-combo.jpg",
                ProductDetails = "{ \"Disk Design\" : \"Cross Drill Slotted\", \" Pad Material\" : \"Ceramic\", \"Construction\" : \"Vented Rotor\", \"Diameter\" : \"10.3 in.\", \"Finish\" : \"Silver Zinc Plated\", \"Hat Finish\" : \"Silver Zinc Plated\", \"Material\" : \"Cast Iron\" }",
                Description = "Our brake disks and pads perform the best togeather. Better stopping distances without locking up, reduced rust and dusk.",
                Inventory = 0,
                LeadTime = 6,
                RecommendationId = 10,
                ProductId = 10
            };

            yield return new Product
            {
                SkuNumber = "BRA-0002",
                Title = "Brake Rotor",
                Category = categoriesMap["Brakes"],
                CategoryId = categoriesMap["Brakes"].CategoryId,
                CategoryName = categoriesMap["Brakes"].Name,
                Price = 18.99M,
                SalePrice = 18.99M,
                CostPrice = 7.99M,
                ProductArtUrl = "product_brakes_disc.jpg",
                ProductDetails = "{ \"Disk Design\" : \"Cross Drill Slotted\",  \"Construction\" : \"Vented Rotor\", \"Diameter\" : \"10.3 in.\", \"Finish\" : \"Silver Zinc Plated\", \"Hat Finish\" : \"Black E-coating\",  \"Material\" : \"Cast Iron\" }",
                Description = "Our Brake Rotor Performs well in wet coditions with a smooth responsive feel. Machined to a high tolerance to ensure all of our Brake Rotors are safe and reliable.",
                Inventory = 4,
                LeadTime = 0,
                RecommendationId = 11,
                ProductId = 11
            };

            yield return new Product
            {
                SkuNumber = "BRA-0003",
                Title = "Brake Disk and Calipers",
                Category = categoriesMap["Brakes"],
                CategoryId = categoriesMap["Brakes"].CategoryId,
                CategoryName = categoriesMap["Brakes"].Name,
                Price = 43.99M,
                SalePrice = 43.99M,
                CostPrice = 38.10M,
                ProductArtUrl = "product_brakes_disc-calipers-red.jpg",
                ProductDetails = "{\"Disk Design\" : \"Cross Drill Slotted\", \" Pad Material\" : \"Carbon Ceramic\",  \"Construction\" : \"Vented Rotor\", \"Diameter\" : \"11.3 in.\", \"Bolt Pattern\": \"6 x 5.31 in.\", \"Finish\" : \"Silver Zinc Plated\",  \"Material\" : \"Carbon Alloy\", \"Includes Brake Pads\" : \"Yes\" }",
                Description = "Upgrading your brakes can increase stopping power, reduce dust and noise. Our Disk Calipers exceed factory specification for the best performance.",
                Inventory = 2,
                LeadTime = 0,
                RecommendationId = 12,
                ProductId = 12
            };

            yield return new Product
            {
                SkuNumber = "BAT-0001",
                Title = "12-Volt Calcium Battery",
                Category = categoriesMap["Batteries"],
                CategoryId = categoriesMap["Batteries"].CategoryId,
                CategoryName = categoriesMap["Batteries"].Name,
                Price = 129.99M,
                SalePrice = 129.99M,
                CostPrice = 80.00M,
                ProductArtUrl = "product_batteries_basic-battery.jpg",
                ProductDetails = "{ \"Type\": \"Calcium\", \"Volts\" : \"12\", \"Weight\" : \"22.9 lbs\", \"Size\" :  \"7.7x5x8.6\", \"Cold Cranking Amps\" : \"510\" }",
                Description = "Calcium is the most common battery type. It is durable and has a long shelf and service life. They also provide high cold cranking amps.",
                Inventory = 9,
                LeadTime = 0,
                RecommendationId = 13,
                ProductId = 13
            };

            yield return new Product
            {
                SkuNumber = "BAT-0002",
                Title = "Spiral Coil Battery",
                Category = categoriesMap["Batteries"],
                CategoryId = categoriesMap["Batteries"].CategoryId,
                CategoryName = categoriesMap["Batteries"].Name,
                Price = 154.99M,
                SalePrice = 154.99M,
                CostPrice = 99.10M,
                ProductArtUrl = "product_batteries_premium-battery.jpg",
                ProductDetails = "{ \"Type\": \"Spiral Coil\", \"Volts\" : \"12\", \"Weight\" : \"20.3 lbs\", \"Size\" :  \"7.4x5.1x8.5\", \"Cold Cranking Amps\" : \"460\" }",
                Description = "Spiral Coil batteries are the preferred option for high performance Vehicles where extra toque is need for starting. They are more resistant to heat and higher charge rates than conventional batteries.",
                Inventory = 3,
                LeadTime = 0,
                RecommendationId = 14,
                ProductId = 14
            };

            yield return new Product
            {
                SkuNumber = "BAT-0003",
                Title = "Jumper Leads",
                Category = categoriesMap["Batteries"],
                CategoryId = categoriesMap["Batteries"].CategoryId,
                CategoryName = categoriesMap["Batteries"].Name,
                Price = 16.99M,
                SalePrice = 16.99M,
                CostPrice = 7.10M,
                ProductArtUrl = "product_batteries_jumper-leads.jpg",
                ProductDetails = "{ \"length\" : \"6ft.\", \"Connection Type\" : \"Alligator Clips\", \"Fit\" : \"Universal\", \"Max Amp's\" : \"750\" }",
                Description = "Battery Jumper Leads have a built in surge protector and a includes a plastic carry case to keep them safe from corrosion.",
                Inventory = 6,
                LeadTime = 0,
                RecommendationId = 15,
                ProductId = 15
            };

            yield return new Product
            {
                SkuNumber = "OIL-0001",
                Title = "Filter Set",
                Category = categoriesMap["Oil"],
                CategoryId = categoriesMap["Oil"].CategoryId,
                CategoryName = categoriesMap["Oil"].Name,
                Price = 28.99M,
                SalePrice = 28.99M,
                CostPrice = 23.99M,
                ProductArtUrl = "product_oil_filters.jpg",
                ProductDetails = "{ \"Filter Type\" : \"Canister and Cartridge\", \"Thread Size\" : \"0.75-16 in.\", \"Anti-Drainback Valve\" : \"Yes\"}",
                Description = "Ensure that your vehicle's engine has a longer life with our new filter set. Trapping more dirt to ensure old freely circulates through your engine.",
                Inventory = 3,
                LeadTime = 0,
                RecommendationId = 16,
                ProductId = 16
            };

            yield return new Product
            {
                SkuNumber = "OIL-0002",
                Title = "Oil and Filter Combo",
                Category = categoriesMap["Oil"],
                CategoryId = categoriesMap["Oil"].CategoryId,
                CategoryName = categoriesMap["Oil"].Name,
                Price = 34.49M,
                SalePrice = 34.49M,
                CostPrice = 19.40M,
                ProductArtUrl = "product_oil_oil-filter-combo.jpg",
                ProductDetails = "{ \"Filter Type\" : \"Canister\", \"Thread Size\" : \"0.75-16 in.\", \"Anti-Drainback Valve\" : \"Yes\", \"Size\" : \"1.1 gal.\", \"Synthetic\" : \"No\" }",
                Description = "This Oil and Oil Filter combo is suitable for all types of passenger and light commercial vehicles. Providing affordable performance through excellent lubrication and breakdown resistance.",
                Inventory = 5,
                LeadTime = 0,
                RecommendationId = 17,
                ProductId = 17
            };

            yield return new Product
            {
                SkuNumber = "OIL-0003",
                Title = "Synthetic Engine Oil",
                Category = categoriesMap["Oil"],
                CategoryId = categoriesMap["Oil"].CategoryId,
                CategoryName = categoriesMap["Oil"].Name,
                Price = 36.49M,
                SalePrice = 36.49M,
                CostPrice = 22.10M,
                ProductArtUrl = "product_oil_premium-oil.jpg",
                ProductDetails = "{ \"Size\" :  \"1.1 Gal.\" , \"Synthetic \" : \"Yes\"}",
                Description = "This Oil is designed to reduce sludge deposits and metal friction throughout your cars engine. Provides performance no matter the condition or temperature.",
                Inventory = 11,
                LeadTime = 0,
                RecommendationId = 18,
                ProductId = 18
            };
        }
    }
}
