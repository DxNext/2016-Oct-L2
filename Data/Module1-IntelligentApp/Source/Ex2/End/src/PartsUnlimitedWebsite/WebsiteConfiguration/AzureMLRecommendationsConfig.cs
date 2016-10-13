// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;

namespace PartsUnlimited.WebsiteConfiguration
{
    public class AzureMLRecommendationsConfig : IAzureMLRecommendationsConfig
    {
        public AzureMLRecommendationsConfig(IConfiguration config)
        {
            AccountEmail = config["AccountEmail"];
            AccountKey = config["AccountKey"];
            ModelId = config["ModelId"];
        }

        public string AccountEmail { get; }
        public string AccountKey { get; }
        public string ModelId { get; }
    }
}