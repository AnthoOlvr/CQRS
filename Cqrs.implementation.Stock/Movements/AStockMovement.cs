using System;

namespace Cqrs.implementation.Stock.Movements
{
    public abstract class AStockMovement
    {
        public Guid Guid { get; }
        public Product Product { get; }
        public DateTime Date { get; }
        public string Label { get; }
        public int Quantity { get; }

        protected AStockMovement(Product product, int quantity, string label, DateTime date)
        {
            Product = product;
            Label = label;
            Date = date;
            Quantity = quantity;
            Guid = Guid.NewGuid();
        }

        public abstract void Execute();

        public override bool Equals(object obj)
        {
            return obj is AStockMovement aStockMovement && aStockMovement.Guid == Guid;
        }

        public override int GetHashCode()
        {
            return new { Guid }.GetHashCode();
        }
    }
}
