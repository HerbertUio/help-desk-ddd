using Domain.Exceptions.Common;

namespace Domain.Exceptions.UserExceptions;

public class EmptyPhoneNumberException: DomainException
{
    
    public EmptyPhoneNumberException(): base("El telefono no puede estar vacío")
    {
    }
}