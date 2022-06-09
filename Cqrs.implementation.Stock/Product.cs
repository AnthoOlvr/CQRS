
using System;

namespace Cqrs.implementation.Stock
{
    public sealed class Product
    {
        public string Ean { get; }

        public ProductStock ProductStock { get; }
        
        public Product(string ean, ProductStock productStock)
        {
            if (ean.Length != 8)
                throw new ArgumentException("Ean must have length of 8 characters!");
            Ean = ean;
            ProductStock = productStock;
            productStock.Product = this;
        }

        public override bool Equals(object obj)
        {
            return obj is Product product && product.Ean == Ean;
        }

        public override int GetHashCode()
        {
            return Ean.GetHashCode();
        }
    }
}
