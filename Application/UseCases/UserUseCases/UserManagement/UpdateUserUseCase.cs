using Application.Dtos.UserDtos;
using Application.Mappings.Extensions;
using Domain.IRepositories;
using Domain.Responses;
using FluentValidation;

namespace Application.UseCases.UserUseCases.UserManagement;

public class UpdateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UpdateUserDto> _updateUserValidator;
    
    public UpdateUserUseCase(IUserRepository userRepository, IValidator<UpdateUserDto> updateUserValidator)
    {
        _userRepository = userRepository;
        _updateUserValidator = updateUserValidator;
    }
    
    public async Task<Result<UserDto>> ExecuteAsync(int id, UpdateUserDto updateUserDto)
    {
        var validationResult = await _updateUserValidator.ValidateAsync(updateUserDto);
        if (!validationResult.IsValid)
        {
            return Result<UserDto>
                .Failure(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList(), "Error de validación");
        }

        try
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return Result<UserDto>.Failure("Usuario no encontrado.", "Error de búsqueda");
            }
            var applyResult = existingUser.ApplyUpdate(updateUserDto);
            if (!applyResult.IsSuccess)
            {
                return Result<UserDto>.Failure(applyResult.Errors, "Error al aplicar cambios");
            }
            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            return Result<UserDto>.Success(updatedUser.ToUserDto(), "Usuario actualizado con éxito!");
        }
        catch (Exception e)
        {
            return Result<UserDto>.Failure($"Error al actualizar usuario {id}: {e.Message}", "Error de Repositorio");
        }
    }
}