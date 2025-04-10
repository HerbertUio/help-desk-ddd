using Domain.IRepositories;
using Domain.Responses;

namespace Application.UseCases.UserUseCases.UserManagement;

public class DeleteUserUseCase
{
    private readonly IUserRepository _userRepository;
    
    public DeleteUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<bool>> ExecuteAsync(int id)
    {
        try
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return Result<bool>.Failure("Usuario no encontrado.", "Error de búsqueda");
            }
            
            await _userRepository.DeleteUserByIdAsync(id);
            return Result<bool>.Success(true, "Usuario eliminado con éxito!");
        }
        catch (Exception ex)
        {
            return Result<bool>
                .Failure($"Error al eliminar el usuario: {ex.Message}", "Error de Repositorio");
        }
    }
}