namespace Cqrs.implementation.Stock.Movements
{
    public abstract class AStockQuery<T>
    {
        public abstract T Query ();
    }
}
