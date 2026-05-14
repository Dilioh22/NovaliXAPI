using FluentValidation;
using RestaurantPOS.Application.DTOs.Products;

namespace RestaurantPOS.Application.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del producto es requerido.")
            .MaximumLength(200).WithMessage("El nombre no puede exceder 200 caracteres.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Debe seleccionar una categoría válida.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.")
            .LessThan(100000).WithMessage("El precio no puede exceder 100,000.");

        RuleFor(x => x.PreparationTime)
            .GreaterThanOrEqualTo(0).When(x => x.PreparationTime.HasValue)
            .WithMessage("El tiempo de preparación no puede ser negativo.");
    }
}
