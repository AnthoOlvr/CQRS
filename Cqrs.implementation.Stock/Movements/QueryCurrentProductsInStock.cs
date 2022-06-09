using System.Collections.Generic;
using Cqrs.implementation.Stock.Ports;

namespace Cqrs.implementation.Stock.Movements
{
    public sealed class QueryCurrentProductsInStock : AStockQuery<IEnumerable<Product>>
    {
        private readonly IStockQueryHandler<QueryCurrentProductsInStock, IEnumerable<Product>> _handler;

        public QueryCurrentProductsInStock(IStockQueryHandler<QueryCurrentProductsInStock, IEnumerable<Product>> handler)
        {
            _handler = handler;
        }

        public override IEnumerable<Product> Query()
        {
           return _handler.QueryStock(this);
        }
    }
}
