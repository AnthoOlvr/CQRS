#nullable enable
using System.Collections.Generic;
using Cqrs.implementation.Stock;

namespace Cqrs.implementation.Infrastructure.Ports
{
    public interface IProductStockRepository
    {
        public void UpdateOrCreateProductStock(Product product);
        public ProductStock? GetProductStockByProductEan(string ean);
        public IEnumerable<Product> GetProductsInStock();
    }
}
