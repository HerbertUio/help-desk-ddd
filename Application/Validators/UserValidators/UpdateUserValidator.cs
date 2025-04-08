using Application.Dtos.UserDtos;
using FluentValidation;

namespace Application.Validators.UserValidators;

public class UpdateUserValidator: AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("El Id es requerido.")
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor que cero.");
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre es requerido.")
            .MinimumLength(2)
            .WithMessage("El nombre debe tener al menos 2 caracteres.")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("El nombre contiene caracteres inválidos.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("El apellido es requerido.")
            .MinimumLength(2)
            .WithMessage("El apellido debe tener al menos 2 caracteres.")
            .Matches(@"^[a-zA-ZÀ-ÿ\s]+$")
            .WithMessage("El apellido contiene caracteres inválidos.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("El número de teléfono es requerido.")
            .Matches(@"^\d{9}$")
            .WithMessage("El número de teléfono debe tener 9 dígitos.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es requerido.")
            .EmailAddress()
            .WithMessage("El email no es válido.");

        RuleFor(p => p.Password)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .MaximumLength(16).WithMessage("La contraseña no puede tener más de 16 caracteres.")
            .Matches(@"[A-Z]+").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches(@"[a-z]+").WithMessage("La contraseña debe contener al menos una letra minúscula.")
            .Matches(@"[0-9]+").WithMessage("La contraseña debe contener al menos un número.")
            .Matches(@"[\!\?\*\.\-_@#\$%^&+=]").WithMessage("La contraseña debe contener al menos un carácter especial.");
        
        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithMessage("El departamento es requerido.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("El rol es requerido.");
        
        RuleFor(x => x.Active)
            .NotEmpty()
            .WithMessage("El estado de actividad es requerido.");

    }
}