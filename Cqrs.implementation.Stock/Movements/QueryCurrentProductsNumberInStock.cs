using Cqrs.implementation.Stock.Ports;

namespace Cqrs.implementation.Stock.Movements
{
    public sealed class QueryCurrentProductsNumberInStock : AStockQuery<int>
    {
        private readonly IStockQueryHandler<QueryCurrentProductsNumberInStock, int> _handler;

        public QueryCurrentProductsNumberInStock(IStockQueryHandler<QueryCurrentProductsNumberInStock, int> handler)
        {
            _handler = handler;
        }

        public override int Query()
        {
            return _handler.QueryStock(this);
        }
    }
}
