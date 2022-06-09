using System;
using Cqrs.implementation.Stock.Ports;

namespace Cqrs.implementation.Stock.Movements
{
    public sealed class QueryProductStockByDate : AStockQuery<int?>
    {
        private readonly IStockQueryHandler<QueryProductStockByDate, int?> _handler;

        public Product Product { get; }
        public DateTime Date { get; }

        public QueryProductStockByDate(IStockQueryHandler<QueryProductStockByDate, int?> handler, Product product, DateTime date)
        {
            _handler = handler;
            Product = product;
            Date = date;
        }


        public override int? Query()
        {
            return _handler.QueryStock(this);
        }
    }
}
