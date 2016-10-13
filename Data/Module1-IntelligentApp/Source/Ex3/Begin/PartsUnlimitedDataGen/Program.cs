namespace PartsUnlimitedDataGen
{
    using Microsoft.Azure.Devices.Client;
    using System;
    using System.Threading;
    using System.Text;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using PartsUnlimited.Models;
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.Azure.Documents.Client.TransientFaultHandling;
    using Microsoft.Azure.Documents;
    using System.Configuration;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Client.TransientFaultHandling.Strategies;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

    public class Program
    {
        static string primarykey = string.Empty;
        static string EndpointUrl = "";
        static string AuthorizationKey = "";
        static IReliableReadWriteDocumentClient docclient;
        static Database docdatabase;
        static DocumentCollection collection;
        static string databaseId;
        static string collectionId;
        static string hubhostName = string.Empty;
        static string deviceId = string.Empty;
        static DeviceClient deviceClient;
        static Random random = new Random();

        static void Main(string[] args)
        {
            /* the only way I can send data is by going through DocDB for my key */
            
            try
            {
                Console.WriteLine("Simulated device\n");
                hubhostName = ConfigurationManager.AppSettings["hubhostname"];
                deviceId = Environment.MachineName;

                string key = GetDocumentDBKey(); //will blow up if no key available
                
                deviceClient = DeviceClient.Create(hubhostName,new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, key));
           
                // Randomly create instances of the store actions, such as add view remove and checkout a product, 
                // convert it into a JSON string and sends to the IoT Hub.
                GenerateRandomEvents();
                Console.ReadLine();

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured: " + e.ToString());
                
            }
            finally
            {
                Console.WriteLine("DataGen has stopped");
            }

            Console.ReadLine();
        }

        static string GetDocumentDBKey()
        {
            EndpointUrl = ConfigurationManager.AppSettings["docdburi"];
            AuthorizationKey = ConfigurationManager.AppSettings["docdbkey"];
            databaseId = ConfigurationManager.AppSettings["databaseId"];
            collectionId = ConfigurationManager.AppSettings["collectionId"];


            docclient = CreateClient(EndpointUrl, AuthorizationKey);

            Database database = docclient.CreateDatabaseQuery().Where(db => db.Id == databaseId).ToArray().FirstOrDefault();
            collection = docclient.CreateDocumentCollectionQuery(database.SelfLink).Where(c => c.Id == collectionId).ToArray().FirstOrDefault();

            var request = docclient.CreateDocumentQuery<DeviceDoc>(collection.SelfLink)
                      .Where(f => f.deviceId == deviceId);

            if (request.ToList().Count > 1) throw new ApplicationException("Too many documents for same DeviceID.  Should only be 1 document");

            if (request.ToList().Count == 0) throw new ApplicationException("You have not registered this device.  Please complete registration first.");

            var deviceDoc = request.ToList().FirstOrDefault();


            return deviceDoc.authentication.symmetricKey.primaryKey;
        }

        private static IReliableReadWriteDocumentClient CreateClient(string uri, string key)
        {
            ConnectionPolicy policy = new ConnectionPolicy()
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            };
            var documentClient = new DocumentClient(new Uri(uri), key, policy);
            var documentRetryStrategy = new DocumentDbRetryStrategy(RetryStrategy.DefaultExponential) { FastFirstRetry = true };
            return documentClient.AsReliable(documentRetryStrategy);
        }

        private static int GetRandomEventNum(int num_choices, List<int> choice_weight)
        {
            int sum_of_weight = 0;
            for (int i = 0; i < num_choices; i++)
            {
                sum_of_weight += choice_weight[i];
            }

            int rnd = random.Next(sum_of_weight);
            for (int i = 0; i < num_choices; i++)
            {
                if (rnd < choice_weight[i])
                    return i;
                rnd -= choice_weight[i];
            }

            return 1;
        }

        private static void GenerateRandomEvents()
        {
            var categories = SampleData.GetCategories().ToList();
            var products = SampleData.GetProducts(categories).ToList();
            var eventTypes = new List<string>() { "add", "view", "checkout", "remove" };
            var eventWeight = new List<int>() {32, 54, 20, 10 };

            /*
            var messagestring = JsonConvert.SerializeObject(products, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            System.IO.File.WriteAllText(@".\productcatalog.json", messagestring);
            */



            while (true)
            {
                var userId = random.Next(1, 250).ToString();
                var eventNum = GetRandomEventNum(eventTypes.Count, eventWeight);
                var randomProduct = products[eventNum];
                var randomEventType = eventTypes[eventNum];
                var eventMessage = new EventMessage();

                if (randomEventType.Equals("checkout"))
                {
                    var qty = random.Next(1, 4);
                    eventMessage = new EventMessage
                    {
                        EventDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        UserId = userId,
                        Type = randomEventType,
                        ProductId = randomProduct.ProductId.ToString(),
                        quantity = qty,
                        Price = (randomProduct.SalePrice * qty)
                    };
                }
                else
                {
                    eventMessage = new EventMessage
                    {
                        EventDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        UserId = userId,
                        Type = randomEventType,
                        ProductId = randomProduct.ProductId.ToString()
                    };
                }

                SendingRandomMessages(eventMessage);
                Thread.Sleep(500);
            }
        }

        private static async void SendingRandomMessages(EventMessage eventMessage)
        {
            try
            {
                var messagestring = JsonConvert.SerializeObject(eventMessage, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() } );

                //EventData data = new EventData(Encoding.UTF8.GetBytes(message));
                var message = new Message(Encoding.ASCII.GetBytes(messagestring));

                await deviceClient.SendEventAsync(message);

                //eventHubClient.Send(data);
                Console.WriteLine("Sent message: {0} at time {1}.",eventMessage.ToString() , DateTime.UtcNow.ToString("yyyyMMdd hh:mm:ss"));
            }
            catch (Exception exception)
            {
                Console.WriteLine(DateTime.Now.ToString() + " > Exception: " + exception.ToString());
            }
        }

       }
    }
