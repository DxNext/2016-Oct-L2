using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RecViewerApp
{
    public partial class _Default : Page
    {
        private const string BaseUri = "https://westus.api.cognitive.microsoft.com/recommendations/v4.0";
        private static RecommendationsApiWrapper recommender = null;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ModelKey_TextChanged(object sender, EventArgs e)
        {
            recommender = new RecommendationsApiWrapper(ModelKey.Text, BaseUri);
            var modelsInfo = recommender.GetModels("Parts Unlimited Store");
            foreach(var model in modelsInfo)
            {
                ModelSelect.Items.Add(new ListItem(model.Name, model.Id));
            }
            

        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}