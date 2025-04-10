using Application.Dtos.UserDtos;
using Application.Mappings.Extensions;
using Domain.Factories;
using Domain.IRepositories;
using Domain.Responses;
using FluentValidation;

namespace Application.UseCases.UserUseCases.UserManagement;

public class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly UserFactory _userFactory;
    private readonly IValidator<CreateUserDto> _createUserValidator;
    
    public CreateUserUseCase(IUserRepository userRepository, UserFactory userFactory, IValidator<CreateUserDto> createUserValidator)
    {
        _userRepository = userRepository;
        _userFactory = userFactory;
        _createUserValidator = createUserValidator;
    }
    
    public async Task<Result<UserDto>> ExecuteAsync(CreateUserDto createUserDto)
    {
        var validationResult = await _createUserValidator.ValidateAsync(createUserDto);
        
        if (!validationResult.IsValid)
        {
            return Result<UserDto>
                .Failure(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList(), "Datos de usuario inválidos.");
        }
        
        bool isUnique = await _userRepository.IsEmailUniqueAsync(createUserDto.Email);
        
        if (!isUnique)
        {
            return Result<UserDto>
                .Failure("El correo electrónico ya está en uso.", "Error de creación de usuario.");
        }
        
        var hashedPasswordString = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password, workFactor: 11);
        
        var userResult = _userFactory.CreateNewUser(
            createUserDto.Name,
            createUserDto.LastName,
            createUserDto.PhoneNumber,
            createUserDto.Email,
            hashedPasswordString,
            createUserDto.DepartmentId,
            createUserDto.Role);
        
        if (!userResult.IsSuccess)
        {
            return Result<UserDto>
                .Failure(userResult.Errors, "Error al crear el usuario.");
        }

        try
        {
            var createdUser = await _userRepository.CreateAsync(userResult.Data!);
            return Result<UserDto>.Success(createdUser.ToUserDto(), "Usuario creado con éxito");
        }
        catch (Exception e)
        {
            return Result<UserDto>
                .Failure($"Error al guardar en base de datos: {e.Message}", "Error de Persistencia");
        }
    }
}