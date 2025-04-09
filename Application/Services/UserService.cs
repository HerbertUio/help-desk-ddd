using Application.Dtos.UserDtos;
using Application.Mappings.Extensions;
using Application.Validators.UserValidators;
using Domain.Enums.UserEnums;
using Domain.Factories;
using Domain.IRepositories;
using Domain.Models;
using Domain.Responses;
using Domain.ValueObjects.UserValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserFactory _userFactory;
    private readonly IValidator<CreateUserDto> _createUserValidator;
    private readonly IValidator<UpdateUserDto> _updateUserValidator;
    private readonly IValidator<UserLoginDto> _loginUserValidator;
    private readonly IConfiguration _configuration;
    
    public UserService(IUserRepository userRepository,
        UserFactory userFactory,
        IValidator<CreateUserDto> createUserValidator,
        IValidator<UpdateUserDto> updateUserValidator,
        IValidator<UserLoginDto> loginUserValidator,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _userFactory = userFactory;
        _createUserValidator = createUserValidator;
        _updateUserValidator = updateUserValidator;
        _loginUserValidator = loginUserValidator;
        _configuration = configuration;
    }

    public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
    {
        var validationResult = await _createUserValidator.ValidateAsync(createUserDto);
        if (!validationResult.IsValid)
        {
            return Result<UserDto>
                .Failure(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList(), "Error de validación");
        }
        var isUnique = await _userRepository.IsEmailUniqueAsync(createUserDto.Email); 
        if (!isUnique)
        {
            return Result<UserDto>
                .Failure($"El email '{createUserDto.Email}' ya está registrado.", "Conflicto");
        }
        
        var hashedPasswordString = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password, workFactor: 11);
        
        var userResult = _userFactory.CrateNewUser(
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
                .Failure(userResult.Errors, "Error al crear el usuario");
        }

        try
        {
            var createdUser = await _userRepository.CreateAsync(userResult.Data);
            return Result<UserDto>.Success(createdUser.ToUserDto(), "Usuario creado con éxito");
        }
        catch (Exception ex)
        {
            return Result<UserDto>
                .Failure($"Error al guardar en base de datos: {ex.Message}", "Error de Persistencia");
        }
    }

    public async Task<Result<UserDto>> UpdateUserAsync(UpdateUserDto updateUserDto)
    {
        var validationResult = await _updateUserValidator.ValidateAsync(updateUserDto);
        if (!validationResult.IsValid)
        {
            return Result<UserDto>
                .Failure(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList(), "Error al actualizar el usuario");
        }
        
    }
}