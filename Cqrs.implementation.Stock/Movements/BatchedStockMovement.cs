using System;
using System.Collections.Generic;

namespace Cqrs.implementation.Stock.Movements
{
    public sealed class BatchedStockMovement
    {
        public IEnumerable<AStockMovement> StockMovements { get; }

        public BatchedStockMovement(IEnumerable<AStockMovement> stockMovements)
        {
            StockMovements = stockMovements;
        }

        public void Execute()
        {
            try
            {
                foreach (var aStockMovement in StockMovements)
                {
                    aStockMovement.Execute();
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
                throw;
            }
        }
    }
}
