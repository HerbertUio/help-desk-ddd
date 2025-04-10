using Application.Dtos.UserDtos;
using Application.Mappings.Extensions;
using Domain.IRepositories;
using Domain.Responses;

namespace Application.UseCases.UserUseCases.UserManagement;

public class GetAllUsersUseCase
{
    private readonly IUserRepository _userRepository;
    
    public GetAllUsersUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<Result<List<UserDto>>> ExecuteAsync()
    {
        try
        {
            var userModels = await _userRepository.GetAllAsync();
            var userDtos = userModels.Select(u => u.ToUserDto()).ToList();
            return Result<List<UserDto>>.Success(userDtos, "Usuarios obtenidos con éxito!");
        }
        catch (Exception ex)
        {
            return Result<List<UserDto>>
                .Failure($"Error al obtener usuarios: {ex.Message}", "Error de Repositorio");
        }
    }
}