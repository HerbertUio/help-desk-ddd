using Application.Dtos.UserDtos;
using Application.Mappings.Extensions;
using Domain.IRepositories;
using Domain.Responses;

namespace Application.UseCases.UserUseCases.UserManagement;

public class GetUserByIdUseCase
{
    private readonly IUserRepository _userRepository;
    
    public GetUserByIdUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<UserDto>> ExecuteAsync(int userId)
    {
        try
        {
            var userModel = await _userRepository.GetUserByIdAsync(userId);
            if (userModel == null)
            {
                return Result<UserDto>.Failure("Usuario no encontrado.", "Error de búsqueda");
            }
            var userDto = userModel.ToUserDto();
            return Result<UserDto>.Success(userDto, "Usuario obtenido con éxito!");
        }
        catch (Exception ex)
        {
            return Result<UserDto>
                .Failure($"Error al obtener el usuario: {ex.Message}", "Error de Repositorio");
        }
    }
}