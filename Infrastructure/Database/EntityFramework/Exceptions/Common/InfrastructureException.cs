namespace Infrastructure.Database.EntityFramework.Exceptions.Common;

public class InfrastructureException: Exception
{
    public InfrastructureException(): base() { }
    public InfrastructureException(string message): base(message) { }
    public InfrastructureException(string message, Exception innerException): base(message, innerException) { }
}