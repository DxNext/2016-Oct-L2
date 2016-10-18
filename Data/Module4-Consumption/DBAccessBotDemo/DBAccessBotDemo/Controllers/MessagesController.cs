using System;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using System.Diagnostics;
using QC = System.Data.SqlClient;

//Simple bot to demo accessing and pulling data from Datawarehouse 
//Two different 'dialogs' (not utilizing luis.ai at the moment) 
    //Product function is triggered with 'profit' in the user message and the product that is looked up must be preceded by ':' (ex. Profit for: Filter Set)
    //Second is triggered with 'products' in the user message with a max profit value preceded by '<' (ex. which products made < 50000) 



namespace DBAccessBotDemo
{
    [Serializable]
    public class EchoDialog : IDialog<object>
    {
        private string connectionString = "Server = tcp:readinesssqlsvr10.database.windows.net,1433; Database = readinessdw; User ID = labuser; Password = labP@ssword1; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;";
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync); //Call out

        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            /*User Dialogs
            -Give me profit on Jumper Leads (Jumper leads will be flexible where it can be any entity)
            -Show me products that have made less then 50,000
             */

            //Connect to DataWarehouse and put content of query into reader
            using (var connection = new QC.SqlConnection(connectionString)) //Call out
            {
                connection.Open();

                //Extract needed data from user text
                var message = await argument;
                string userText = message.Text;

                if (userText.Contains("profit") && userText.Contains(":"))
                {
                    string title = userText.Split(':')[1];
                    title = title.Trim(' ');
                    using (var Command = new QC.SqlCommand())
                    {
                        Command.Connection = connection;
                        Command.CommandType = System.Data.CommandType.Text; //Call out
                        Command.CommandText = @"select a.Profit, b.title from adw.ProfitableProducts a INNER JOIN adw.DimProductCatalog b ON a.productId=b.productId WHERE b.title='" + title + "' ";

                        QC.SqlDataReader reader = Command.ExecuteReader();

                        //await context.PostAsync("Output: ");

                        while (reader.Read())
                        {
                            await context.PostAsync(String.Format("Profit for {0}: ${1}", title, reader.GetDouble(0).ToString()));  //Call out
                            // await context.PostAsync(reader.GetDouble(0).ToString());
                        }
                        if (reader.HasRows == false)
                        {
                            await context.PostAsync(String.Format("{0} Not in the database", title));
                        }
                    }
                }
                else if (userText.Contains("<") || userText.Contains("less than"))
                {
                    //Grab integer value 
                    //String res = userText.Split('<')[1];

                    //int lessThen = Int32.Parse(res.Trim(' '));
                    int lessThen = Int32.Parse(userText.Split('<')[1].Trim(' ').Replace(",", ""));
                    using (var Command = new QC.SqlCommand())
                    {
                        Command.Connection = connection;
                        Command.CommandType = System.Data.CommandType.Text; //Call out
                        Command.CommandText = @"select a.*, b.title from adw.ProfitableProducts a INNER JOIN adw.DimProductCatalog b ON a.productId=b.productID WHERE a.Profit<'" + lessThen + "' ";

                        QC.SqlDataReader reader = Command.ExecuteReader();

                        String result = String.Format("Products that made less then {0}: \n", lessThen);
                        while (reader.Read())
                        {
                            result = result + reader.GetString(3) + ", Profit: "+ reader.GetDouble(2)+ "; ";
                        }
                        result = result.Substring(0, result.Length - 2);
                        await context.PostAsync(result);

                    }
                }
                else if (userText.Contains("most profitable"))
                {
                    //Grab integer value 
                    //String res = userText.Split('<')[1];

                    //int lessThen = Int32.Parse(res.Trim(' '));
                    using (var Command = new QC.SqlCommand())
                    {
                        Command.Connection = connection;
                        Command.CommandType = System.Data.CommandType.Text; //Call out
                        Command.CommandText = @"select top 1 a.*, b.title from adw.ProfitableProducts a INNER JOIN adw.DimProductCatalog b ON a.productId=b.productID ORDER BY a.Profit DESC";

                        QC.SqlDataReader reader = Command.ExecuteReader();

                        String result = String.Format("Most Profitable Product : ");
                        int loopCount = 0;
                        while (reader.Read())
                        {
                            result = result + reader.GetString(3) + "; Profit: "+ reader.GetDouble(2);
                            loopCount++;
                        }
                        if(loopCount==1)
                        {
                            await context.PostAsync(result);
                        }
                        else
                        {
                            await context.PostAsync("You entered a command that I cannot process please enter pruduct or profit request.");
                        }

                    }
                }
                else if (userText.Contains("hi") || userText.Contains("hello") || userText.Contains("Hello") || userText.Contains("Hi"))
                {
                    await context.PostAsync("Hello! Please ask me about the profitability of your products");
                }
                else
                {
                    await context.PostAsync("You entered a command that I cannot process please enter pruduct or profit request.");
                }

                context.Wait(MessageReceivedAsync); //Call out

            }
        }

        [BotAuthentication]
        public class MessagesController : ApiController
        {
            /// <summary>
            /// POST: api/Messages
            /// receive a message from a user and send replies
            /// </summary>
            /// <param name="activity"></param>
            [ResponseType(typeof(void))]
            public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
            {
                // check if activity is of type message
                if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
                {
                    await Conversation.SendAsync(activity, () => new EchoDialog());
                }
                else
                {
                    HandleSystemMessage(activity);
                }
                return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
            }

            private Activity HandleSystemMessage(Activity message)
            {
                if (message.Type == ActivityTypes.DeleteUserData)
                {
                    // Implement user deletion here
                    // If we handle user deletion, return a real message
                }
                else if (message.Type == ActivityTypes.ConversationUpdate)
                {
                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels
                }
                else if (message.Type == ActivityTypes.ContactRelationUpdate)
                {
                    // Handle add/remove from contact lists
                    // Activity.From + Activity.Action represent what happened
                }
                else if (message.Type == ActivityTypes.Typing)
                {
                    // Handle knowing tha the user is typing
                }
                else if (message.Type == ActivityTypes.Ping)
                {
                }

                return null;
            }
        }
    }
}
