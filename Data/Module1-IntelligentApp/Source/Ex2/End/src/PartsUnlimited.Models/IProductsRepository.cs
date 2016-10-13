namespace PartsUnlimited.Models
{
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IProductsRepository
    {
        DocumentClient Client { get; }

        Database Database { get; }

        DocumentCollection Collection { get; }

        Document GetDocumentById(int id);

        Product GetById(int id);

        IEnumerable<Product> Find(Expression<Func<Product, bool>> predicate);

        IQueryable<T> Set<T>();

        Task CreateAsync(Product product);

        Task UpdateAsync(Product product);

        Task CreateOrUpdateAsync(Product product);

        Task DeleteAsync(Product product);
    }
}
