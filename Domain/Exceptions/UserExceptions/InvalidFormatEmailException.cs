using Domain.Exceptions.Common;

namespace Domain.Exceptions.UserExceptions;

public class InvalidFormatEmailException: DomainException
{
    public InvalidFormatEmailException(): base("El formato del email no es válido")
    {
    }
    
}