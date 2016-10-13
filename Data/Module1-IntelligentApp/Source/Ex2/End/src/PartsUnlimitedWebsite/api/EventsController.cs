namespace PartsUnlimited.api
{
    using System.Text;
    using Microsoft.AspNet.Mvc;
    using Microsoft.ServiceBus.Messaging;

    [Route("api/events")]
    public class EventsController : Controller
    {
        private static string eventHubName = "eventhubdatamodule";
        private static string connectionString = "{SendRule Connection String}";

        // POST api/values
        [HttpPost]
        public void Post([FromForm]string serializedEventMessage)
        {
            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionString, eventHubName);
            var eventData = new EventData(Encoding.UTF8.GetBytes(serializedEventMessage));
            eventHubClient.Send(eventData);
        }
    }
}
