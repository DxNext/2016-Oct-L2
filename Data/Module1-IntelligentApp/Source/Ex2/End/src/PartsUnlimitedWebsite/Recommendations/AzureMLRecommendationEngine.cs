// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using PartsUnlimited.Telemetry;
using PartsUnlimited.WebsiteConfiguration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace PartsUnlimited.Recommendations
{
    /// <summary>
    /// This class implements Azure ML Frequently Bought Together recommendation engine
    /// Details can be found at https://datamarket.azure.com/dataset/amla/mba
    /// </summary>
    public class AzureMLRecommendationEngine : IRecommendationEngine
    {
        private readonly IAzureMLRecommendationsConfig _config;
        private readonly IAzureMLAuthenticatedHttpClient _client;
        private readonly ITelemetryProvider _telemetry;

        private class AzureMLFrequentlyBoughtTogetherServiceResponse
        {
            public List<string> ItemSet { get; set; }
            public int Value { get; set; }
        }

        public AzureMLRecommendationEngine(IAzureMLRecommendationsConfig configFile, IAzureMLAuthenticatedHttpClient httpClient, ITelemetryProvider telemetryProvider)
        {
            _config = configFile;
            _client = httpClient;
            _telemetry = telemetryProvider;
        }

        public async Task<IEnumerable<string>> GetRecommendationsAsync(string itemId)
        {
            //The Azure ML service takes in a recommendation model Id (trained ahead of time) and a product id
            string uri = "https://api.datamarket.azure.com/amla/recommendations/v3/ItemRecommend?modelId=%27{0}%27&itemIds=%27{1}%27&numberOfResults={2}&includeMetadata=false&apiVersion=%271.0%27";
            var requestString = String.Format(uri, _config.ModelId, itemId, 8);

            try
            {
                //The Recommendations API returns a list of recommended items based on a list of input items. Rating of the recommendation; higher number means higher confidence.
                var response = await _client.GetStringAsync(requestString);
                var recommendedProducts = ExtractProductIdsFromResponse(response);
                
                //When there is no recommendation, we return an empty list 
                if (recommendedProducts == null)
                {
                    return Enumerable.Empty<string>();
                }
                else
                {
                    return recommendedProducts;
                }
            }
            catch (HttpRequestException e)
            {
                _telemetry.TrackException(e);

                return Enumerable.Empty<string>();
            }
        }

        private IEnumerable<string> ExtractProductIdsFromResponse(string response)
        {
            var nodeList = XmlUtils.ExtractXmlElementList(response, "//a:entry/a:content/m:properties");

            var recommendedItems = XmlUtils.ExtractRecommendedItems(nodeList);

            return recommendedItems.Select(i => i.Id); //.ToList();
        }

        /// <summary>
        /// Utility class holding a recommended item information.
        /// </summary>
        public class RecommendedItem
        {
            public string Name { get; set; }
            public string Rating { get; set; }
            public string Reasoning { get; set; }
            public string Id { get; set; }

            public override string ToString()
            {
                return string.Format("Name: {0}, Id: {1}, Rating: {2}, Reasoning: {3}", Name, Id, Rating, Reasoning);
            }
        }


        private class XmlUtils
        {
            /// <summary>
            /// extract a single xml node from the given stream, given by the xPath
            /// </summary>
            /// <param name="xmlStream"></param>
            /// <param name="xPath"></param>
            /// <returns></returns>
            internal static XmlNode ExtractXmlElement(Stream xmlStream, string xPath)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlStream);
                //Create namespace manager
                var nsmgr = CreateNamespaceManager(xmlDoc);

                var node = xmlDoc.SelectSingleNode(xPath, nsmgr);
                return node;
            }

            private static XmlNamespaceManager CreateNamespaceManager(XmlDocument xmlDoc)
            {
                var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
                nsmgr.AddNamespace("a", "http://www.w3.org/2005/Atom");
                nsmgr.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
                nsmgr.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
                return nsmgr;
            }

            /// <summary>
            /// extract the xml nodes from the given stream, given by the xPath
            /// </summary>
            /// <param name="xmlStream"></param>
            /// <param name="xPath"></param>
            /// <returns></returns>
            internal static XmlNodeList ExtractXmlElementList(string xmlStream, string xPath)
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlStream);
                var nsmgr = CreateNamespaceManager(xmlDoc);
                var nodeList = xmlDoc.SelectNodes(xPath, nsmgr);
                return nodeList;
            }

            /// <summary>
            /// Utility method to extract the recommended item from a xml recommendation result 
            /// </summary>
            /// <param name="nodeList">the xml element containing the recommended items.</param>
            /// <returns>a collection of recommended item or empty list id sh</returns>
            internal static IEnumerable<RecommendedItem> ExtractRecommendedItems(XmlNodeList nodeList)
            {
                var recoList = new List<RecommendedItem>();
                foreach (var node in (nodeList))
                {
                    var item = new RecommendedItem();
                    //cycle through the recommended items
                    foreach (var child in ((XmlElement)node).ChildNodes)
                    {
                        //cycle through properties
                        var nodeName = ((XmlNode)child).LocalName;
                        switch (nodeName)
                        {
                            case "Id":
                                item.Id = ((XmlNode)child).InnerText;
                                break;
                            case "Name":
                                item.Name = ((XmlNode)child).InnerText;
                                break;
                            case "Rating":
                                item.Rating = ((XmlNode)child).InnerText;
                                break;
                            case "Reasoning":
                                item.Reasoning = ((XmlNode)child).InnerText;
                                break;
                        }
                    }
                    recoList.Add(item);
                }
                return recoList;
            }
        }
    }
}