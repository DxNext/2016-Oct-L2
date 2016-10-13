using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client.TransientFaultHandling;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Client.TransientFaultHandling.Strategies;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace CreateDeviceIdentity
{
    class Program
    {
        static RegistryManager registryManager;
        static string hubconnectionString = string.Empty;
        private static string EndpointUrl;
        private static string AuthorizationKey;
        private static string collectionId;
        private static string databaseId;
        static IReliableReadWriteDocumentClient docclient;
        static Database docdatabase;
        static DocumentCollection collection;

        private static async Task AddDeviceAsync()
        {
            bool shallIAddToDocDB = false;
            string deviceId = Environment.MachineName;
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
                shallIAddToDocDB = true;
            }
            catch (DeviceAlreadyExistsException)
            {
                Console.WriteLine("Device Exists already so retrieving the device details from registry");
                device = await registryManager.GetDeviceAsync(deviceId);
                shallIAddToDocDB = false;
            }

            /*
            now let's store in DocDB.  A good location to store this information due to the readability
            */
            if (shallIAddToDocDB)
            {
                AddDeviceToDocDB(device);
            }
            Console.WriteLine("Generated device key (copy this or look in DocDB): {0}", device.Authentication.SymmetricKey.PrimaryKey);
            Console.ReadLine();
        }

        private static void AddDeviceToDocDB(Device device)
        {
            var devicedoc = JsonConvert.SerializeObject(device);

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(devicedoc)))
            {
                docclient.CreateDocumentAsync(collection.SelfLink, Document.LoadFrom<Document>(ms));

            }

            Console.WriteLine("device {0} as been added to docDB", device.Id);
        }

        static void Main(string[] args)
        {
            
            hubconnectionString = ConfigurationManager.AppSettings["hubconnectionstring"];
            EndpointUrl = ConfigurationManager.AppSettings["docdburi"];
            AuthorizationKey = ConfigurationManager.AppSettings["docdbkey"];
            databaseId = ConfigurationManager.AppSettings["databaseId"];
            collectionId = ConfigurationManager.AppSettings["collectionId"];



            docclient = CreateClient(EndpointUrl, AuthorizationKey);

            docdatabase = docclient.CreateDatabaseQuery().Where(db => db.Id == databaseId).ToArray().FirstOrDefault();
            collection = docclient.CreateDocumentCollectionQuery(docdatabase.SelfLink).Where(c => c.Id == collectionId).ToArray().FirstOrDefault();


            registryManager = RegistryManager.CreateFromConnectionString(hubconnectionString);
            AddDeviceAsync().Wait();
            Console.ReadLine();
        }

        private static IReliableReadWriteDocumentClient CreateClient(string endpointUrl, string authorizationKey)
        {
            ConnectionPolicy policy = new ConnectionPolicy()
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            };
            var documentClient = new DocumentClient(new Uri(endpointUrl), authorizationKey, policy);
            var documentRetryStrategy = new DocumentDbRetryStrategy(RetryStrategy.DefaultExponential) { FastFirstRetry = true };
            return documentClient.AsReliable(documentRetryStrategy);
        }
    }
}
