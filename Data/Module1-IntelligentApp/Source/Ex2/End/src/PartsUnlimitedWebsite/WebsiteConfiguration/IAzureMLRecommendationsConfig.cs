// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PartsUnlimited.WebsiteConfiguration
{
    public interface IAzureMLRecommendationsConfig
    {
        string AccountEmail { get; }
        string AccountKey { get; }
        string ModelId { get; }
    }
}