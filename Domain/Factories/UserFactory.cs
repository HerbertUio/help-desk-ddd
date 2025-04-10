using Domain.Enums.UserEnums;
using Domain.Models;
using Domain.Responses;
using Domain.ValueObjects.UserValueObjects;

namespace Domain.Factories;

public class UserFactory
{
    public Result<UserModel> CreateNewUser(
        string name,
        string lastName,
        string phoneString,
        string emailString,
        string hashedPasswordString, 
        int departmentId,
        string roleString,
        bool active = true)
    {
        var validationErrors = new List<string>();
        var emailResult = UserEmail.Create(emailString);
        var phoneResult = UserPhoneNumber.Create(phoneString);
        var passwordResult = HashedPassword.Create(hashedPasswordString);
        var roleParseResult = Enum.TryParse<UserRole>(roleString, true, out var parsedRole)
                               && Enum.IsDefined(parsedRole)
                               && parsedRole != UserRole.Undefined;
        try
        {
            var userModel = new UserModel(
                0,
                name.Trim(),
                lastName.Trim(),
                phoneResult.Data,
                emailResult.Data,
                passwordResult.Data,
                departmentId,
                parsedRole,
                active
            );
            
            return Result<UserModel>.Success(userModel, "Modelo de usuario creado correctamente.");
        }
        catch (ArgumentException ex) 
        {
             return Result<UserModel>.Failure(new List<string>{ ex.Message }, "Error en la construcción del modelo de usuario.");
        }
    }

}