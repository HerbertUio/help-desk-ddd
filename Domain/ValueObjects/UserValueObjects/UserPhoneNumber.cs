using System.Text.RegularExpressions;
using Domain.Exceptions.UserExceptions;

namespace Domain.ValueObjects.UserValueObjects;

public class UserPhoneNumber
{
    public string Value { get; set; }
    private UserPhoneNumber(string value) => Value = value;
    
    private static bool IsValidPhoneNumber(string number)
    {
        return Regex.Match(number, @"^(\+?\s?[0-9\s?]+)$").Success;
    }
    
    public static UserPhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new EmptyPhoneNumberException();
        }
        value = value.Trim();
        if (!IsValidPhoneNumber(value))
        {
            throw new InvalidFormatPhoneNumberException();
        }
        return new UserPhoneNumber(value);
    }
    public static implicit operator string(UserPhoneNumber userPhoneNumber) => userPhoneNumber.Value;
    public override string ToString() => Value;
}