using System.Text.RegularExpressions;
using Domain.Responses;

namespace Domain.ValueObjects.UserValueObjects;

public class UserPhoneNumber
{
    public string Value { get; set; }
    private UserPhoneNumber(string value) => Value = value;
    
    private static bool IsValidPhoneNumber(string number)
    {
        return Regex.Match(number, @"^(\+?\s?[0-9\s?]+)$").Success;
    }
    
    public static Result<UserPhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<UserPhoneNumber>
                .Failure("El número de teléfono no puede estar vacío.", "Error de Validación");
        }
        value = value.Trim();
        if (!IsValidPhoneNumber(value))
        {
            return Result<UserPhoneNumber>.Failure("El formato del número de teléfono no es válido.", "Error de Validación");
        }
        return Result<UserPhoneNumber>.Success(new UserPhoneNumber(value), "Número de teléfono creado correctamente.");
    }
    public static implicit operator string(UserPhoneNumber userPhoneNumber) => userPhoneNumber.Value;
    public override string ToString() => Value;
}