#nullable enable
using Cqrs.implementation.Stock.Movements;

namespace Cqrs.implementation.Stock.Ports
{
    public interface IStockMovementHandler<in T> where T : AStockMovement
    {
        void HandleStockMovement(T aStockMovement);
        void CompleteMovement(AStockMovement aStockMovement);
    }

    public interface IStockQueryHandler<in T, out TS> where T : AStockQuery<TS?>
    {
        TS? QueryStock(T aStockQuery);
    }

    public interface IInventoryMovementHandler
    {
        void CompleteInventoryMovement(InventoryMovement inventoryMovement);
        void HandleInventoryMovement(InventoryMovement inventoryMovement);
    }
}
