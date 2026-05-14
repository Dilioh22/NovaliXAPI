using FluentValidation;
using RestaurantPOS.Application.DTOs.Orders;

namespace RestaurantPOS.Application.Validators;

public class CreateOrderValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderValidator()
    {
        RuleFor(x => x.OrderType)
            .NotEmpty().WithMessage("El tipo de orden es requerido.")
            .Must(t => t == "DineIn" || t == "TakeOut" || t == "Delivery")
            .WithMessage("El tipo de orden debe ser DineIn, TakeOut o Delivery.");

        RuleFor(x => x.TableId)
            .GreaterThan(0).When(x => x.TableId.HasValue)
            .WithMessage("El ID de mesa debe ser válido.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("La orden debe tener al menos un item.");

        RuleForEach(x => x.Items).SetValidator(new AddOrderItemValidator());
    }
}

public class AddOrderItemValidator : AbstractValidator<AddOrderItemDto>
{
    public AddOrderItemValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("El ID del producto debe ser válido.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.")
            .LessThanOrEqualTo(100).WithMessage("La cantidad no puede exceder 100.");
    }
}
