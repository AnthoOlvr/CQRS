#nullable enable
using Cqrs.implementation.Stock.Ports;

namespace Cqrs.implementation.Stock.Movements
{
    public sealed class QueryCurrentProductStock : AStockQuery<ProductStock?>
    {
        private readonly IStockQueryHandler<QueryCurrentProductStock, ProductStock?> _handler;

        public string Ean { get; }

        public QueryCurrentProductStock(IStockQueryHandler<QueryCurrentProductStock, ProductStock?> handler, string ean)
        {
            _handler = handler;
            Ean = ean;
        }


        public override ProductStock? Query()
        {
            return _handler.QueryStock(this);
        }
    }
}
