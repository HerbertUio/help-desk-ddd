using Application.Dtos.UserDtos;
using FluentValidation;

namespace Application.Validators.UserValidators;

public class LoginUserValidator: AbstractValidator<UserLoginDto>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es requerido.")
            .EmailAddress()
            .WithMessage("El email no es válido.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es requerida.")
            .MinimumLength(8)
            .WithMessage("La contraseña debe tener al menos 8 caracteres.");
    }
}