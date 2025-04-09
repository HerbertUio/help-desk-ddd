using Domain.Enums.UserEnums;
using Domain.Models;
using Domain.Responses;
using Domain.ValueObjects.UserValueObjects;

namespace Domain.Factories;

public class UserFactory
{
    public Result<UserModel> CrateNewUser(
        string name,
        string lastName,
        string phoneNumber,
        string email,
        string password,
        int departmentId,
        string role,
        bool active = true)
    {
        var emailResult = UserEmail.Create(email);
        var phoneNumberResult = UserPhoneNumber.Create(phoneNumber);
        var roleResult = Enum.TryParse<UserRole>(role, out var userRole);
        
        var user = new UserModel(
            0,
            name,
            lastName,
            phoneNumberResult,
            emailResult,
            password,
            departmentId,
            userRole,
            active);
        return Result<UserModel>.Success(user, "Modelo de usuario creado correctamente.");
    }
    
    public Result<UserModel> UpdateUser(
        int id,
        string name,
        string lastName,
        string phoneNumber,
        string email,
        string password,
        int departmentId,
        string role,
        bool active = true)
    {
        var emailResult = UserEmail.Create(email);
        var phoneNumberResult = UserPhoneNumber.Create(phoneNumber);
        var roleResult = Enum.TryParse<UserRole>(role, out var userRole);
        
        var user = new UserModel(
            id,
            name,
            lastName,
            phoneNumberResult,
            emailResult,
            password,
            departmentId,
            userRole,
            active);
        return Result<UserModel>.Success(user, "Modelo de usuario actualizado correctamente.");
    }
}