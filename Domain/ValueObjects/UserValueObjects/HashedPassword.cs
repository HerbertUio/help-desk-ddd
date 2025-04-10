using Domain.Responses;

namespace Domain.ValueObjects.UserValueObjects;

public record HashedPassword
{
    public string Value { get; }

    private HashedPassword(string value)
    {
        Value = value;
    }

    public static Result<HashedPassword> Create(string? hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            return Result<HashedPassword>.Failure("El hash de contraseña no puede estar vacío.", "Error de Validación");
        }
        return Result<HashedPassword>.Success(new HashedPassword(hash), "Hash de contraseña válido.");
    }

    public static implicit operator string(HashedPassword hashedPassword) => hashedPassword.Value;

    public override string ToString() => Value; 

}