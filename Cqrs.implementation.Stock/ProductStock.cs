#nullable enable
using System;

namespace Cqrs.implementation.Stock
{
    public sealed class ProductStock
    {
        public Guid Guid { get; }

        public Product? Product { get; set; }

        public int Quantity { get; set; }

        public ProductStock()
        {
            Guid = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return obj is ProductStock productStock && productStock.Guid == Guid;
        }

        public override int GetHashCode()
        {
            return new { Id = Guid }.GetHashCode();
        }
    }
}
