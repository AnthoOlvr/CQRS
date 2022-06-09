using System;
using Cqrs.implementation.Stock.Ports;

namespace Cqrs.implementation.Stock.Movements
{
    public sealed class PurchaseMovement : AStockMovement
    {
        private readonly IStockMovementHandler<PurchaseMovement> _handler;


        public PurchaseMovement(IStockMovementHandler<PurchaseMovement> handler, Product product, int quantity, string label, DateTime date) : base(product, quantity, label, date)
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
