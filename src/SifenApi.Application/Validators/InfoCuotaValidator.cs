using FluentValidation;
using SifenApi.Application.DTOs;

namespace SifenApi.Application.Validators;

public class InfoCuotaValidator : AbstractValidator<InfoCuotaDto>
{
    public InfoCuotaValidator()
    {
        RuleFor(x => x.Moneda)
            .NotEmpty()
            .Length(3)
            .WithMessage("La moneda debe tener 3 caracteres");

        RuleFor(x => x.Monto)
            .GreaterThan(0)
            .WithMessage("El monto de la cuota debe ser mayor a cero");

        RuleFor(x => x.Vencimiento)
            .GreaterThan(DateTime.Now.Date)
            .WithMessage("La fecha de vencimiento debe ser futura");
    }
}