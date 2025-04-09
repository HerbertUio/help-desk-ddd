using Application.Dtos.UserDtos;
using Domain.Enums.UserEnums;
using Domain.Models;

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
}