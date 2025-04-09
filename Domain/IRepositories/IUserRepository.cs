using Domain.IRepositories.Common;
using Domain.Models;

namespace Domain.IRepositories;

public interface IUserRepository: IGenericRepository<UserModel>
{
    Task<List<UserModel>> GetAllAsync();
    Task<UserModel?> GetUserByIdAsync(int id);
    Task<UserModel?> GetUserByEmailAsync(string email);
    Task<bool> DeleteUserByIdAsync(int id);
    Task<bool> IsEmailUniqueAsync(string email);
    
}