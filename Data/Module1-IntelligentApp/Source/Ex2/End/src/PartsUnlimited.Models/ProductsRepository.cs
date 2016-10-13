namespace PartsUnlimited.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using Newtonsoft.Json;
    using System.Linq.Expressions;
    using System.Net;
    using System.Globalization;

    public class ProductsRepository : IProductsRepository
    {
        private string endpoint;
        private string authKey;
        private string databaseId;
        private string collectionId;

        private DocumentClient client;
        private Database database;
        private DocumentCollection collection;

        public ProductsRepository(string endpoint, string authKey, string databaseId, string collectionId)
        {
            this.endpoint = endpoint;
            this.authKey = authKey;
            this.databaseId = databaseId;
            this.collectionId = collectionId;
        }

        public DocumentClient Client
        {
            get
            {
                if (client == null)
                {
                    Uri endpointUri = new Uri(this.endpoint);
                    client = new DocumentClient(endpointUri, this.authKey);
                }

                return client;
            }
        }

        public Database Database
        {
            get
            {
                if (database == null)
                {
                    database = ReadOrCreateDatabase();
                }

                return database;
            }
        }

        public DocumentCollection Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = ReadOrCreateCollection(Database.SelfLink);
                }

                return collection;
            }
        }

        public Document GetDocumentById(int id)
        {
            return this.Set<Document>()
                    .Where(d => d.Id == id.ToString())
                    .AsEnumerable()
                    .FirstOrDefault();
        }

        public Product GetById(int id)
        {
            return this.Set<Product>()
                    .Where(d => d.ProductId == id)
                    .AsEnumerable()
                    .FirstOrDefault();
        }

        public IEnumerable<Product> Find(Expression<Func<Product, bool>> predicate)
        {
            return this.Set<Product>()
                    .Where(predicate)
                    .AsEnumerable();
        }

        public IQueryable<T> Set<T>()
        {
            return this.Client.CreateDocumentQuery<T>(this.Collection.SelfLink);
        }

        public async Task CreateAsync(Product product)
        {
            if (product.ProductId == 0)
            {
                product.ProductId = GenerateProductId();
            }

            var response = await this.Client.CreateDocumentAsync(this.Collection.SelfLink, product);
            this.EnsureSuccessStatusCode(response);
        }

        public async Task UpdateAsync(Product product)
        {
            Document doc = this.GetDocumentById(product.ProductId);

            if (doc == null)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "Cannot find a product with id '{0}'", product.ProductId));
            }

            var response = await this.Client.ReplaceDocumentAsync(doc.SelfLink, product);
            this.EnsureSuccessStatusCode(response);
        }

        public async Task CreateOrUpdateAsync(Product product)
        {
            ResourceResponse<Document> response;
            Document doc = this.GetDocumentById(product.ProductId);

            if (doc != null)
            {
                response = await this.Client.ReplaceDocumentAsync(doc.SelfLink, product);
            }
            else
            {
                if (product.ProductId == 0)
                {
                    product.ProductId = GenerateProductId();
                }

                response = await this.Client.CreateDocumentAsync(this.Collection.SelfLink, product);
            }

            this.EnsureSuccessStatusCode(response);
        }

        public async Task DeleteAsync(Product product)
        {
            Document doc = this.GetDocumentById(product.ProductId);

            if (doc != null)
            {
                await this.Client.DeleteDocumentAsync(doc.SelfLink);
            }
        }

        private Database ReadOrCreateDatabase()
        {
            var db = this.Client.CreateDatabaseQuery()
                            .Where(d => d.Id == this.databaseId)
                            .AsEnumerable()
                            .FirstOrDefault();

            if (db == null)
            {
                db = this.Client.CreateDatabaseAsync(new Database { Id = this.databaseId }).Result;
            }

            return db;
        }

        private DocumentCollection ReadOrCreateCollection(string databaseLink)
        {
            var col = this.Client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == this.collectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (col == null)
            {
                col = this.Client.CreateDocumentCollectionAsync(databaseLink, new DocumentCollection { Id = this.collectionId }).Result;
            }

            return col;
        }

        private void EnsureSuccessStatusCode(ResourceResponse<Document> response)
        {
            if (response.StatusCode < HttpStatusCode.OK || response.StatusCode > (HttpStatusCode)299)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "There was a problem while storing the product. The Response status code does not indicate success: {0}.", response.StatusCode));
            }
        }

        private int GenerateProductId()
        {
            // This is mock product id generation logic.
            var products = this.Set<Product>().ToList();
            return (products.Any()) ? products.Max(p => p.ProductId) + 1 : 1;
        }
    }
}
