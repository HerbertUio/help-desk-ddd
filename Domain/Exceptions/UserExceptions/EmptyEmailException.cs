using Domain.Exceptions.Common;

namespace Domain.Exceptions.UserExceptions;

public class EmptyEmailException: DomainException
{
    public EmptyEmailException(): base("El email no puede estar vacío")
    {
    }
}