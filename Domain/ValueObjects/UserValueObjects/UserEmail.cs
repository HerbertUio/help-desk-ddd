using System.Text.RegularExpressions;
using Domain.Responses;


namespace Domain.ValueObjects.UserValueObjects;

public record UserEmail
{
    public string Value { get; set; }
    private UserEmail(string emailValue) => Value = emailValue;
    
    private static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
    
    public static Result<UserEmail>  Create(string emailValue)
    {
        if (string.IsNullOrWhiteSpace(emailValue))
        {
            return Result<UserEmail>.Failure("El email no puede estar vacío.", "Error de Validación");
        }
        emailValue = emailValue.Trim();
        if (!IsValidEmail(emailValue))
        {
            return Result<UserEmail>.Failure("El formato del email no es válido.", "Error de Validación");
        }
        return Result<UserEmail>.Success(new UserEmail(emailValue), "Email creado correctamente.");
    }
    public static implicit operator string(UserEmail email)
    {
        return email.Value;
    }
    public override string ToString() => Value;
}