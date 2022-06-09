using System;
using Cqrs.implementation.Stock.Ports;

namespace Cqrs.implementation.Stock.Movements
{
    public sealed class InventoryMovement : AStockMovement
    {
        private readonly IInventoryMovementHandler _handler;
        public InventoryMovement(IInventoryMovementHandler handler, Product product, int quantity) : base (product, quantity, "Inventory", DateTime.UtcNow)
        {
            _handler = handler;
            if (quantity < 0)
                throw new ArgumentException("Inventory movements are unique and cannot be negative");
        }

        public override void Execute()
        {
            _handler.HandleInventoryMovement(this);
            _handler.CompleteInventoryMovement(this);
        }
    }
}
