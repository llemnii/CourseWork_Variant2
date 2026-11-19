namespace Domain.Core.Exceptions;

public class InvalidDiscountException : Exception
{
    public InvalidDiscountException(string message)
        : base(message)
    {
    }
}
