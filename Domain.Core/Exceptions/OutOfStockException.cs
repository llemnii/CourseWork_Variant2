namespace Domain.Core.Exceptions;

public class OutOfStockException : Exception
{
    public OutOfStockException(string productName)
        : base($"Товару «{productName}» недостатньо на складі.")
    {
    }
}
