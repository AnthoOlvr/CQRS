using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Cqrs.implementation.Stock
{
    public sealed class Event
    {
        public int id { get; set; }
        public Guid DataSourceGuid { get; set; }
        public DateTime RecordingDate { get; set; }
        public JsonDocument Datas { get; set; }
    }
}
