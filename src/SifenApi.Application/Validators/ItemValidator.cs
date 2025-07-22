using FluentValidation;
using SifenApi.Application.DTOs;

namespace SifenApi.Application.Validators;

public class ItemValidator : AbstractValidator<ItemDto>
{
    public ItemValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Código del item es requerido");

        RuleFor(x => x.Descripcion)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Descripción del item es requerida");

        RuleFor(x => x.UnidadMedida)
            .GreaterThan(0)
            .WithMessage("Unidad de medida inválida");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0)
            .WithMessage("La cantidad debe ser mayor a cero");

        RuleFor(x => x.PrecioUnitario)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El precio unitario no puede ser negativo");

        RuleFor(x => x.IvaTipo)
            .IsInEnum()
            .WithMessage("Tipo de IVA inválido");

        RuleFor(x => x.Iva)
            .InclusiveBetween(0, 100)
            .WithMessage("La tasa de IVA debe estar entre 0 y 100");

        RuleFor(x => x.Descuento)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Descuento.HasValue)
            .WithMessage("El descuento no puede ser negativo");

        RuleFor(x => x.Descuento)
            .Must((item, descuento) => !descuento.HasValue || 
                                       descuento.Value <= (item.Cantidad * item.PrecioUnitario))
            .WithMessage("El descuento no puede ser mayor al precio total del item");

        When(x => x.SectorAutomotor != null, () =>
        {
            RuleFor(x => x.SectorAutomotor!.Chasis)
                .NotEmpty()
                .Length(17)
                .WithMessage("El chasis debe tener 17 caracteres");

            RuleFor(x => x.SectorAutomotor!.Año)
                .InclusiveBetween(1900, DateTime.Now.Year + 1)
                .When(x => x.SectorAutomotor!.Año.HasValue)
                .WithMessage("Año del vehículo inválido");
        });
    }
}