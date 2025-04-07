using System.Text.RegularExpressions;
using Domain.Exceptions.UserExceptions;
using Domain.Responses;
using Domain.Models;

namespace Domain.ValueObjects.UserValueObjects;

public record UserEmail
{
    public string Value { get; set; }
    private UserEmail(string emailValue) => Value = emailValue;
    
    private static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
    
    public static UserEmail Create(string emailValue)
    {
        if (string.IsNullOrWhiteSpace(emailValue))
        {
            throw new EmptyEmailException();
        }
        emailValue = emailValue.Trim();
        if (!IsValidEmail(emailValue))
        {
            throw new InvalidFormatEmailException();
        }
        return new UserEmail(emailValue);
    }
    public static implicit operator string(UserEmail email)
    {
        return email.Value;
    }
    public override string ToString() => Value;
}