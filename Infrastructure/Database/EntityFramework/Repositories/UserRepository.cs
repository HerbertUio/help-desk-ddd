using Domain.IRepositories;
using Domain.Models;
using Infrastructure.Database.EntityFramework.Context;
using Infrastructure.Database.EntityFramework.Entities;
using Infrastructure.Database.EntityFramework.Exceptions;
using Infrastructure.Database.EntityFramework.Mappings.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Infrastructure.Database.EntityFramework.Repositories;

public class UserRepository: IUserRepository
{
    private readonly HelpDeskDbContext _context;
    private readonly DbSet<UserEntity> _users;
    private readonly ILogger<UserRepository> _logger;
    
    public async Task<UserModel> CreateAsync(UserModel model)
    {
        if(model == null) throw new ArgumentNullException(nameof(model));
        var entity = model.ToEntity();
        await _users.AddAsync(entity);
        try
        {
            await _context.SaveChangesAsync();
            _logger?.LogInformation("Usuario creado con ID: {UserId}", entity.Id);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        { throw new DuplicateEntityException($"Conflicto de valor único al crear usuario (ej. email '{entity.Email}').", ex); }
        catch (DbUpdateException ex)
        { throw new DatabaseOperationException($"Error DB al crear usuario: {ex.Message}", ex); }
        catch (Exception ex)
        { throw new DatabaseOperationException($"Error inesperado al crear usuario: {ex.Message}", ex); }
        return entity.ToModel();
    }

    public async Task<UserModel> UpdateAsync(UserModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        var entity = await _users.FindAsync(model.Id);
        if (entity == null) throw new EntityNotFoundException($"Usuario con ID {model.Id} no encontrado.");
        
        entity.Name = model.Name;
        entity.LastName = model.LastName;
        entity.PhoneNumber = model.PhoneNumber;
        entity.Email = model.Email;
        if (!string.IsNullOrWhiteSpace(model.Password) && entity.Password != model.Password)
        { entity.Password = model.Password; }
        entity.DepartmentId = model.DepartmentId;
        entity.Role = model.Role.ToString();
        entity.Active = model.Active;
        try
        {
            await _context.SaveChangesAsync();
            _logger?.LogInformation("Usuario actualizado con ID: {UserId}", entity.Id);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        { throw new DuplicateEntityException($"Conflicto de valor único al actualizar usuario.", ex); }
        catch (DbUpdateConcurrencyException ex)
        { throw new DatabaseOperationException($"Conflicto de concurrencia al actualizar usuario {model.Id}.", ex); }
        catch (DbUpdateException ex)
        { throw new DatabaseOperationException($"Error DB al actualizar usuario {model.Id}.", ex); }
        catch (Exception ex)
        { throw new DatabaseOperationException($"Error inesperado al actualizar usuario {model.Id}.", ex); }

        return entity.ToModel();
    }

    public async Task<List<UserModel>> GetAllAsync()
    {
        try
        {
            var entities = await _users.AsNoTracking().ToListAsync();
            return entities.Select(entity => entity.ToModel()).ToList();
        }
        catch(Exception ex) { throw new DatabaseOperationException("Error al consultar todos los usuarios.", ex); }
    }

    public async Task<UserModel?> GetUserByIdAsync(int id)
    {
        try
        {
            var entity = await _users.FindAsync(id);
            return entity?.ToModel();
        }
        catch(Exception ex) { throw new DatabaseOperationException($"Error al consultar usuario con ID {id}.", ex); }
    }

    public async Task<UserModel?> GetUserByEmailAsync(string email)
    {
        try
        {
            string lowerEmail = email.ToLowerInvariant();
            var entity = await _users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == lowerEmail);
            return entity?.ToModel(); 
        }
        catch(Exception ex) { throw new DatabaseOperationException($"Error al consultar usuario con email {email}.", ex); }
    }

    public async Task<bool> DeleteUserByIdAsync(int id)
    {
        var entity = await _users.FindAsync(id);
        if (entity == null)
        {
            return false; 
        }
        _users.Remove(entity);
        try
        {
            var changes = await _context.SaveChangesAsync();
            if (changes > 0) _logger?.LogInformation("Usuario eliminado con ID: {UserId}", id);
            return changes > 0;
        }
        catch (DbUpdateException ex) { throw new DatabaseOperationException($"Error DB al eliminar usuario {id}.", ex); }
        catch(Exception ex) { throw new DatabaseOperationException($"Error inesperado al eliminar usuario {id}.", ex); }
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        try
        {
            string lowerEmail = email.ToLowerInvariant();
            return !await _users.AnyAsync(u => u.Email.ToLower() == lowerEmail);
        }
        catch(Exception ex) { throw new DatabaseOperationException($"Error al verificar unicidad de email {email}.", ex); }
    }
    public async Task<UserModel?> GetUserByNameAsync(string name) 
    {
        try
        {
            string lowerName = name.ToLowerInvariant();
            var entity = await _users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Name.ToLower() == lowerName); 
            return entity?.ToModel();
        }
        catch(Exception ex) { throw new DatabaseOperationException($"Error al consultar usuario con nombre {name}.", ex); }
    }
}