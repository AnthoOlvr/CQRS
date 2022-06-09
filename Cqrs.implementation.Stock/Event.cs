using System;
using System.Collections.Generic;
using System.Text;

namespace Cqrs.implementation.Stock
{
    public sealed class Event
    {
        public int id { get; set; }
        public int ProductStockId { get; set; }
    }
}
