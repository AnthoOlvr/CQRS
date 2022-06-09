using System;
using Cqrs.implementation.Stock.Ports;

namespace Cqrs.implementation.Stock.Movements
{
    public sealed class OrderMovement : AStockMovement
    {
        private readonly IStockMovementHandler<OrderMovement> _handler;

        public OrderMovement(IStockMovementHandler<OrderMovement> handler, Product product, int quantity, string label, DateTime date) : base (product, -quantity, label, date)
        {
            _handler = handler;
        }

        public override void Execute()
        {
            _handler.HandleStockMovement(this);
            _handler.CompleteMovement(this);
        }
    }
}
