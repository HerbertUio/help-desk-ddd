using Application.Dtos.UserDtos;
using Domain.Enums.UserEnums;
using Domain.Models;
using Domain.ValueObjects.UserValueObjects;
using Infrastructure.Database.EntityFramework.Entities;

namespace Infrastructure.Database.EntityFramework.Mappings.Extensions;

public static class UserExtension
{
    public static UserEntity ToEntity(this UserModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        return new UserEntity()
        {
            Id = model.Id,
            Name = model.Name,
            LastName = model.LastName,
            PhoneNumber = model.PhoneNumber.Value,
            Email = model.Email.Value,
            DepartmentId = model.DepartmentId,
            Role = model.Role.ToString(),
            Active = model.Active
        };
    }

    public static UserModel ToModel(this UserEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        try
        {
            var emailValueObject = UserEmail.Create(entity.Email);
            var hashedPassword = HashedPassword.Create(entity.Password);
            var phoneValueObject = UserPhoneNumber.Create(entity.PhoneNumber);
            var roleParseResult = Enum.TryParse<UserRole>(entity.Role, true, out var parsedRole)
                                  && Enum.IsDefined(parsedRole);
            if (!roleParseResult)
            {
                throw new ArgumentException($"El rol '{entity.Role}' no es válido.");
            }

            return new UserModel(
                entity.Id,
                entity.Name,
                entity.LastName,
                phoneValueObject.Data,
                emailValueObject.Data,
                hashedPassword.Data,
                entity.DepartmentId,
                parsedRole,
                entity.Active
            );
        }
        catch (Exception e)
        {
            throw new ArgumentException($"Error al convertir la entidad a modelo: {e.Message}", e);
        }
    }
    
}