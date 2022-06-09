#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Cqrs.implementation.Infrastructure.Ports;
using Cqrs.implementation.Stock;

namespace Cqrs.implementation.Infrastructure
{
    public class ProductStockRepository : IProductStockRepository
    {
        private readonly ICollection<ProductStock> _stocks = new List<ProductStock>();


        public void UpdateOrCreateProductStock(Product product)
        {
            var stock = _stocks.FirstOrDefault(x => x.Equals(product.ProductStock));
            if (stock == null)
            {
                _stocks.Add(product.ProductStock);
                return;
            }


            stock.Quantity = product.ProductStock.Quantity;
            stock.Product = product;
        }

        public ProductStock? GetProductStockByProductEan(string ean)
        {
            return _stocks.FirstOrDefault(x => x.Product?.Ean == ean);
        }

        public IEnumerable<Product> GetProductsInStock()
        {
            return _stocks.Where(x => x.Product != null && x.Quantity > 0).Select(x => x.Product)!;
        }
    }
}
