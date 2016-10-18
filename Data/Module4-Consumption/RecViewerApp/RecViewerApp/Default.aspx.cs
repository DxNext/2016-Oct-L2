using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace RecViewerApp
{
    public class ProductDetailsDisplay
    {
        public string productID { get; set; }
        public string title { get; set; }

        public string productArtUrl { get; set; }
    }
    public partial class _Default : Page
    {
        private const string BaseUri = "https://westus.api.cognitive.microsoft.com/recommendations/v4.0";
        private static RecommendationsApiWrapper recommender = null;
        private static string modelId = null;
        private static long activeBuildId = 0;
        private static IEnumerable<ProductDetailsDisplay> products = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var rawProductData = File.ReadAllText(Server.MapPath("~/Resources/productcatalog.json"));
                products = JsonConvert.DeserializeObject<IEnumerable<ProductDetailsDisplay>>(rawProductData);
                productdetails.DataSource = products;
                productdetails.DataBind();
            }
                
        }

        protected void ModelKey_TextChanged(object sender, EventArgs e)
        {
            recommender = new RecommendationsApiWrapper(ModelKey.Text, BaseUri);
            var modelsInfo = recommender.GetModels("Parts Unlimited Store");
            foreach(var model in modelsInfo.Models)
            {
                ModelSelect.Items.Add(new ListItem(model.Name, model.Id+"_"+model.ActiveBuildId.ToString()));
            }
            

        }

        protected void ModelSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            var modelInfo = (string)ModelSelect.SelectedValue;
            modelId = modelInfo.Split('_')[0];
            activeBuildId = Int64.Parse(modelInfo.Split('_')[1]);
        }

        protected void productdetails_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            /// Gets the product ID, 
            if (e.CommandName.Equals("Click"))
            {
                var itemIndex = e.Item.ItemIndex;
                var modelInfo = (string)ModelSelect.SelectedValue;
                modelId = modelInfo.Split('_')[0];
                activeBuildId = Int64.Parse(modelInfo.Split('_')[1]);
                var recs = recommender.GetRecommendations(modelId,activeBuildId,products.ElementAt(itemIndex).productID,5);

                recommendations.DataSource = recs.RecommendedItemSetInfo;
                recommendations.DataBind();
            }
        }
    }
}