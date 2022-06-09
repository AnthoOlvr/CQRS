#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Cqrs.implementation.Infrastructure.Ports;
using Cqrs.implementation.Stock;
using Cqrs.implementation.Stock.Movements;
using Cqrs.implementation.Stock.Ports;

namespace Cqrs.implementation.Infrastructure
{
    public class StockHandler :
        IStockMovementHandler<OrderMovement>,
        IStockMovementHandler<PurchaseMovement>,
        IStockQueryHandler<QueryProductStockByDate, int?>,
        IStockQueryHandler<QueryProductStocksInterval, int>,
        IStockQueryHandler<QueryCurrentProductStock, ProductStock?>, 
        IStockQueryHandler<QueryCurrentProductsInStock, IEnumerable<Product>>,
        IStockQueryHandler<QueryCurrentProductsNumberInStock, int>,
        IInventoryMovementHandler

    {
        //  Start implementation of a snapshot for the ProductStock state at the end of the day

        private readonly IProductStockRepository _productStockRepository;
        private readonly ICollection<AStockMovement> _aStockMovements = new List<AStockMovement>();
        private readonly object _stockMovementsMemLock = new object();
        private volatile InventoryMovement? _lastInventoryMovement;

        private void ThrowIfMovementDateIsEarlierThanInventory(AStockMovement aStockMovement)
        {
            if (_lastInventoryMovement == null)
                return;
            if (aStockMovement.Date <= _lastInventoryMovement.Date)
                throw new InvalidOperationException("It is not possible to add a movement at a date earlier than or equal to that of an inventory.");
        }

        public StockHandler(IProductStockRepository productStockRepository)
        {
            _productStockRepository = productStockRepository;
        }

        public void HandleStockMovement(PurchaseMovement purchaseMovement)
        {
            ThrowIfMovementDateIsEarlierThanInventory(purchaseMovement);
            purchaseMovement.Product.ProductStock.Quantity += purchaseMovement.Quantity;
            _productStockRepository.UpdateOrCreateProductStock(purchaseMovement.Product);
        }

        public void HandleStockMovement(OrderMovement orderMovement)
        {
            ThrowIfMovementDateIsEarlierThanInventory(orderMovement);
            orderMovement.Product.ProductStock.Quantity += orderMovement.Quantity;
            _productStockRepository.UpdateOrCreateProductStock(orderMovement.Product);
        }

        public void HandleInventoryMovement(InventoryMovement inventoryMovement)
        {
            inventoryMovement.Product.ProductStock.Quantity = inventoryMovement.Quantity;
            _productStockRepository.UpdateOrCreateProductStock(inventoryMovement.Product);
        }

        public void CompleteMovement(AStockMovement aStockMovement)
        {
            lock (_stockMovementsMemLock)
            {
                _aStockMovements.Add(aStockMovement);
            }
        }

        public void CompleteInventoryMovement(InventoryMovement inventoryMovement)
        {
            lock (_stockMovementsMemLock)
            {
                _aStockMovements.Add(inventoryMovement);
                _lastInventoryMovement = inventoryMovement;
            }
        }

        public int? QueryStock(QueryProductStockByDate queryProductStockByDate)
        {
            lock (_stockMovementsMemLock)
            {
                return _aStockMovements.FirstOrDefault(x => x.Product.ProductStock.Equals(queryProductStockByDate.Product.ProductStock) && x.Date == queryProductStockByDate.Date)?.Quantity;
            }
        }

        public int QueryStock(QueryProductStocksInterval queryProductStocksInterval)
        {
            lock (_stockMovementsMemLock)
            {
                var filteredMovements = _aStockMovements
                    .Where(x => x.Product.Equals(queryProductStocksInterval.Product))
                    .Where(x => x.Date >= queryProductStocksInterval.DateStart && x.Date <= queryProductStocksInterval.DateEnd)
                    .OrderBy(x => x.Date)
                    .ToList();
                var lastMovement = filteredMovements.Last();
                var firstMovement = filteredMovements.First();

                if (lastMovement.Quantity > 0)
                    return lastMovement.Quantity - firstMovement.Quantity;

                return lastMovement.Quantity + firstMovement.Quantity;
            }
        }

        public ProductStock? QueryStock(QueryCurrentProductStock queryCurrentProductStock)
        {
            return _productStockRepository.GetProductStockByProductEan(queryCurrentProductStock.Ean);
        }

        public IEnumerable<Product> QueryStock(QueryCurrentProductsInStock queryCurrentProductsInStock)
        {
            return _productStockRepository.GetProductsInStock();
        }

        public int QueryStock(QueryCurrentProductsNumberInStock queryCurrentProductsNumberInStock)
        {
            return _productStockRepository.GetProductsInStock().Count();
        }
    }
}
