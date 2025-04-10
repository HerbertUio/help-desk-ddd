using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Dtos.UserDtos;
using Application.Mappings.Extensions;
using Domain.IRepositories;
using Domain.Models;
using Domain.Responses;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.UseCases.UserUseCases.UserAuthentication;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UserLoginDto> _userLoginValidator;
    private readonly IConfiguration _configuration;
    
    public LoginUseCase(IUserRepository userRepository, IValidator<UserLoginDto> userLoginValidator, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _userLoginValidator = userLoginValidator;
        _configuration = configuration;
    }

    public async Task<Result<ResponseLoginDto>> ExecuteAsync(UserLoginDto userLoginDto)
    {
        var validationResult = await _userLoginValidator.ValidateAsync(userLoginDto);
        if (!validationResult.IsValid)
        {
            return Result<ResponseLoginDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage).ToList(), "Datos de login inválidos.");
        }

        UserModel userModel;
        try
        {
            userModel = await _userRepository.GetUserByEmailAsync(userLoginDto.Email);
        }
        catch (Exception ex)
        {
            return Result<ResponseLoginDto>
                .Failure($"Error al consultar usuario: {ex.Message}", "Error de Repositorio.");
        }
        if (userModel == null || !userModel.Active)
        {
            return Result<ResponseLoginDto>
                .Failure("Autenticación fallida o usuario inactivo.", "Error de Inicio de Sesión.");
        }
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, userModel.Password);
        if (!isPasswordValid)
        {
            return Result<ResponseLoginDto>.Failure("Autenticación fallida.", "Error de autenticación.");
        }
        var token = GenerateJwtToken(userModel);
        var userData = userModel.ToDataUserDto(); 
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