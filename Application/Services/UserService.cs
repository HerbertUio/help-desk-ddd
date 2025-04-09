using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
using Microsoft.IdentityModel.Tokens;

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

    public async Task<Result<UserDto>> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var validationResult = await _updateUserValidator.ValidateAsync(updateUserDto);
        if (!validationResult.IsValid)
        {
            return Result<UserDto>
                .Failure(validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList(), "Error al actualizar el usuario");
        }

        var existingUser = await _userRepository.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            return Result<UserDto>
                .Failure($"No se encontró el usuario con ID '{id}'", "No encontrado");
        }

        var isUnique = await _userRepository.IsEmailUniqueAsync(updateUserDto.Email);
        if (!isUnique)
        {
            return Result<UserDto>
                .Failure($"El email '{updateUserDto.Email}' ya está registrado.", "Conflicto");
        }

        var hashedPasswordString = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password, workFactor: 11);
        var userResult = _userFactory.UpdateUser(
            id,
            updateUserDto.Name,
            updateUserDto.LastName,
            updateUserDto.PhoneNumber,
            updateUserDto.Email,
            hashedPasswordString,
            updateUserDto.DepartmentId,
            updateUserDto.Role);
        if (!userResult.IsSuccess)
        {
            return Result<UserDto>
                .Failure(userResult.Errors, "Error al actualizar el usuario");
        }

        try
        {
            var updatedUser = await _userRepository.UpdateAsync(userResult.Data);
            return Result<UserDto>.Success(updatedUser.ToUserDto(), "Usuario actualizado con éxito");
        }
        catch (Exception ex)
        {
            return Result<UserDto>
                .Failure($"Error al guardar en base de datos: {ex.Message}", "Error de Persistencia");
        }
    }
    
    public async Task<Result<List<UserDto>>> GetAllAsync()
    {
        try
        {
            var userModels = await _userRepository.GetAllAsync();
            var userDtos = userModels.Select(u => u.ToUserDto()).ToList();
            return Result<List<UserDto>>.Success(userDtos, "Usuarios obtenidos con éxito!");
        }
        catch (Exception ex)
        {
            return Result<List<UserDto>>.Failure($"Error al obtener usuarios: {ex.Message}", "Error de Repositorio");
        }
    }
    public async Task<Result<UserDto?>> GetUserById(int id)
    {
        try
        {
            var userModel = await _userRepository.GetUserByIdAsync(id);
            if (userModel == null)
            {
                return Result<UserDto?>.Failure("Usuario no encontrado.", "No Encontrado");
            }
            return Result<UserDto?>.Success(userModel.ToUserDto(), "Usuario obtenido con éxito.");
        }
        catch (Exception ex)
        {
            return Result<UserDto?>.Failure($"Error al obtener usuario {id}: {ex.Message}", "Error de Repositorio");
        }
    }
    public async Task<Result<bool>> DeleteUser(int id)
    {
        try
        {
            var deleted = await _userRepository.DeleteUserByIdAsync(id);
            if (!deleted)
            {
                return Result<bool>.Failure($"No se pudo eliminar el usuario con ID {id} (puede que no exista).", "Fallo Eliminación");
            }
            return Result<bool>.Success(true, "Usuario eliminado con éxito.");
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure($"Error al eliminar usuario {id}: {ex.Message}", "Error de Repositorio");
        }
    }
    public async Task<Result<ResponseLoginDto>> Login(UserLoginDto loginDto)
    {
        var validationResult = await _loginUserValidator.ValidateAsync(loginDto);
        if (!validationResult.IsValid)
        {
            return Result<ResponseLoginDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList(), "Datos de login inválidos.");
        }

        UserModel? user;
        try
        {
            user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
        }
        catch (Exception ex)
        {
            return Result<ResponseLoginDto>.Failure($"Error al consultar usuario: {ex.Message}", "Error de Repositorio");
        }


        if (user == null || !user.Active)
        {
            return Result<ResponseLoginDto>.Failure("Autenticación fallida.", "Error de autenticación.");
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);
        if (!isPasswordValid)
        {
            return Result<ResponseLoginDto>.Failure("Autenticación fallida.", "Error de autenticación.");
        }

        var token = GenerateJwtToken(user);
        var userData = user.ToDataUserDto();
        var response = new ResponseLoginDto { User = userData, Token = token };
        return Result<ResponseLoginDto>.Success(response, "Login exitoso.");
    }

    private string GenerateJwtToken(UserModel user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expiryMinutes = Convert.ToInt32(jwtSettings["ExpiryMinutes"] ?? "60");

        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("Configuración JWT incompleta.");
        }

        var keyBytes = Encoding.ASCII.GetBytes(secretKey);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim> {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email.Value),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.Name} {user.LastName}"),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}