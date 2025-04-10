using Application.Dtos.UserDtos;
using Domain.Enums.UserEnums;
using Domain.Models;
using Domain.Responses;
using Domain.ValueObjects.UserValueObjects;

namespace Application.Mappings.Extensions;

public static class UserExtend
{
    public static UserDto ToUserDto(this UserModel userModel)
    {
        if (userModel == null) return null!;
        return new UserDto
        {
            Name = userModel.Name,
            LastName = userModel.LastName,
            PhoneNumber = userModel.PhoneNumber.Value,
            Email = userModel.Email.Value,
            DepartmentId = userModel.DepartmentId,
            Role = userModel.Role.ToString(),
            Active = userModel.Active
        };
    }
    public static DataUserDto ToDataUserDto(this UserModel userModel)
    {
        if (userModel == null) return null!;
        return new DataUserDto
        {
            Id = userModel.Id,
            Name = userModel.Name,
            LastName = userModel.LastName,
            PhoneNumber = userModel.PhoneNumber.Value,
            Email = userModel.Email.Value,
            DepartmentId = userModel.DepartmentId,
            Role = userModel.Role.ToString()
        };
    }

    public static Result<bool> ApplyUpdate(this UserModel existingUser, UpdateUserDto updateDto)
    {
        if (existingUser == null || updateDto == null)
        {
            return Result<bool>.Failure("Usuario o datos de actualización no válidos.", "Error de actualización");
        }
        
        var validationResult = ValidateUpdateData(updateDto);
        if (!validationResult.IsSuccess)
        {
            return validationResult;
        }

        try
        {
            var phoneResult = UserPhoneNumber.Create(updateDto.PhoneNumber);
            var parsedRole = Enum.Parse<UserRole>(updateDto.Role, true);
            
            existingUser.UpdateUserModel(
                updateDto.Name.Trim(),
                updateDto.LastName.Trim(),
                phoneResult.Data,
                existingUser.Email,
                existingUser.Password,
                updateDto.DepartmentId,
                parsedRole,
                updateDto.Active
            );
            
            return Result.Success("Actualización aplicada al modelo en memoria.");
        }
        catch (Exception e)
        {
            return Result.Failure($"Error al aplicar detalles de actualización al modelo: {e.Message}", "Error Interno Modelo");
        }
    }

    private static Result<bool> ValidateUpdateData(UpdateUserDto updateDto)
    {
        var validationErrors = new List<string>();
        
        var phoneResult = UserPhoneNumber.Create(updateDto.PhoneNumber);
        if (phoneResult == null)
        {
            validationErrors.Add("Número de teléfono no válido.");
        }
        
        var roleParseResult = Enum.TryParse<UserRole>(updateDto.Role, true, out var parsedRole)
                            && Enum.IsDefined(parsedRole)
                            && parsedRole != UserRole.Undefined;
        if (!roleParseResult)
        {
            validationErrors.Add("Rol de usuario no válido.");
        }
        
        return validationErrors.Any() 
            ? Result<bool>.Failure(validationErrors, "Error de validación") 
            : Result<bool>.Success(true, "Datos de actualización válidos.");
    }
}