using Domain.Exceptions.Common;

namespace Domain.Exceptions.UserExceptions;

public class InvalidFormatPhoneNumberException: DomainException
{
    public InvalidFormatPhoneNumberException(): base("El formato del numero de telefono no es válido")
    {
    }
}