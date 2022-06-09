using System;
using Cqrs.implementation.Stock.Ports;

namespace Cqrs.implementation.Stock.Movements
{
    public sealed class QueryProductStocksInterval : AStockQuery<int>
    {
        private readonly IStockQueryHandler<QueryProductStocksInterval, int> _handler;

        public Product Product { get; }
        public DateTime DateStart { get; }
        public DateTime DateEnd { get; }

        public QueryProductStocksInterval(IStockQueryHandler<QueryProductStocksInterval, int> handler, Product product, DateTime dateStart, DateTime dateEnd)
        {
            _handler = handler;
            Product = product;
            DateStart = dateStart;
            DateEnd = dateEnd;
        }


        public override int Query()
        {
            return _handler.QueryStock(this);
        }
    }
}
