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

    public class RecDetailsDisplay
    {
        public string productID { get; set; }
        public string title { get; set; }

        public string productArtUrl { get; set; }

        public string reasoning { get; set; }

        public double rating { get; set; }
    }

    public partial class _Default : Page
    {
        private const string BaseUri = "https://westus.api.cognitive.microsoft.com/recommendations/v4.0";
        private static RecommendationsApiWrapper recommender = null;
        private static string modelId = null;
        private static long activeBuildId = 0;
        private static List<ProductDetailsDisplay> products = new List<ProductDetailsDisplay>();
        private static List<RecDetailsDisplay> recs = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var rawProductData = File.ReadAllLines(Server.MapPath("~/Resources/productcatalog.json"));
                foreach(var productData in rawProductData)
                {
                    var product = JsonConvert.DeserializeObject<ProductDetailsDisplay>(productData);
                    products.Add(product);
                }
                
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
                recs = new List<RecDetailsDisplay>();
                var itemIndex = e.Item.ItemIndex;
                var modelInfo = (string)ModelSelect.SelectedValue;

                if(modelInfo.Split('_')[1].Length>0)
                {
                    modelId = modelInfo.Split('_')[0];
                    activeBuildId = Int64.Parse(modelInfo.Split('_')[1]);
                    var recos = recommender.GetRecommendations(modelId, activeBuildId, products.ElementAt(itemIndex).productID, 5);

                    var rawRecs = recos.RecommendedItemSetInfo;
                    foreach (var rec in rawRecs)
                    {
                        var recId = rec.Items.ElementAt(0).Id;
                        var recNum = Int32.Parse(recId) - 1;
                        var prodToRec = new RecDetailsDisplay();
                        var prod = products.ElementAt(recNum);
                        prodToRec.productID = prod.productID;
                        prodToRec.title = prod.title;
                        prodToRec.productArtUrl = prod.productArtUrl;
                        prodToRec.rating = rec.Rating;
                        prodToRec.reasoning = rec.Reasoning.ElementAt(0);
                        // var prod = products.Where(x => x.productID.Equals(recId)).Select(x => x);
                        recs.Add(prodToRec); //Since the product ID is at a zero-based index and the list is at a 1-based index
                    }


                    recommendations.DataSource = recs;
                    recommendations.DataBind();



                    recDisplay.Visible = true;
                }
                
                
            }
        }

        protected void recdetails_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.Equals("Click"))
            {
                
                var prodRec = recs.Where(x => x.productID.Equals(e.CommandArgument)).ElementAt(0);
                confidenceLabel.InnerText = prodRec.rating.ToString();
                reasoningLabel.InnerText = prodRec.reasoning;

                conf.Visible = true;
                reason.Visible = true;
            }
        }

        protected void productImages_Click(object sender, ImageClickEventArgs e)
        {
            var modelInfo = (string)ModelSelect.SelectedValue;

            if (modelInfo.Split('_')[1].Length > 0)
            {
                ImageButton selected_img = (ImageButton)sender;

                for (int i= 0; i<productdetails.Items.Count; i++)
                {
                    ImageButton _imgBtn = (ImageButton)(productdetails.Items[i].FindControl("productImages"));
                    // Check if this Image Button is the currently Selected one; If it is,
                    //Change the color as required, otherwise change to something else.
                    if (_imgBtn == selected_img)
                    {
                        selected_img.BorderColor = System.Drawing.Color.IndianRed;
                        selected_img.BorderStyle = BorderStyle.Solid;
                    }
                    else
                    {
                        _imgBtn.BorderStyle = BorderStyle.None;
                    }

                }
            }
            
        }
    }
}