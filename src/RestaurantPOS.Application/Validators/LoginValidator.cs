using FluentValidation;
using RestaurantPOS.Application.DTOs.Auth;

namespace RestaurantPOS.Application.Validators;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido.")
            .EmailAddress().WithMessage("El email no tiene un formato válido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .MinimumLength(4).WithMessage("La contraseña debe tener al menos 4 caracteres.");
    }
}
