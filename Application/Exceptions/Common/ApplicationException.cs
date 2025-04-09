namespace Application.Exceptions.Common;

public class ApplicationException: Exception
{
    public ApplicationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public ApplicationException(string? message) : base(message)
    {
    }
}