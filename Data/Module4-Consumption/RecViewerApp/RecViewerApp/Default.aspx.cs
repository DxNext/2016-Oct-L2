using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RecViewerApp
{
    public class ProductDetailsDisplay
    {
        public string productID { get; set; }
        public string productsrc { get; set; }
        
    }
    public partial class _Default : Page
    {
        private const string BaseUri = "https://westus.api.cognitive.microsoft.com/recommendations/v4.0";
        private static RecommendationsApiWrapper recommender = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Get the list from products file
                List<ProductDetailsDisplay> products = new List<ProductDetailsDisplay>() {
                new ProductDetailsDisplay { productsrc="images/product_brakes_disc.jpg",productID="1" },
                new ProductDetailsDisplay { productsrc="images/product_lighting_bugeye-headlight.jpg",productID="2" },
                new ProductDetailsDisplay { productsrc="images/product_wheel_tyre-wheel-combo-pack.jpg",productID="3" },
                new ProductDetailsDisplay { productsrc="images/product_oil_oil-filter-combo.jpg",productID="4" },
                new ProductDetailsDisplay { productsrc="images/product_lighting_lightbulb.jpg",productID="5" },
                new ProductDetailsDisplay { productsrc="images/product_wheel_tyre-wheel-combo.jpg",productID="6" },
                new ProductDetailsDisplay { productsrc="images/product_wheel_tyre-rim-chrome-combo.jpg",productID="7" },

            };
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
                ModelSelect.Items.Add(new ListItem(model.Name, model.Id));
            }
            

        }

        protected void ModelSelect_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void productdetails_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            /// Gets the product ID, 
            switch (e.CommandName)
            {
                case "Click":
                    Response.Write(e.CommandArgument.ToString());
                    break;
            }
        }
    }
}