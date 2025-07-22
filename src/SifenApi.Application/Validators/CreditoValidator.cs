using FluentValidation;
using SifenApi.Application.DTOs;

namespace SifenApi.Application.Validators;

public class CreditoValidator : AbstractValidator<CreditoDto>
{
    public CreditoValidator()
    {
        RuleFor(x => x.Tipo)
            .IsInEnum()
            .WithMessage("Tipo de crédito inválido");

        RuleFor(x => x.Cuotas)
            .GreaterThan(0)
            .When(x => x.Cuotas.HasValue)
            .WithMessage("El número de cuotas debe ser mayor a cero");

        RuleFor(x => x.InfoCuotas)
            .NotEmpty()
            .When(x => x.Cuotas.HasValue && x.Cuotas.Value > 0)
            .WithMessage("Debe especificar la información de las cuotas");

        RuleFor(x => x.InfoCuotas)
            .Must((credito, cuotas) => cuotas == null || cuotas.Count == credito.Cuotas)
            .When(x => x.Cuotas.HasValue && x.InfoCuotas != null)
            .WithMessage("El número de cuotas informadas debe coincidir con el número de cuotas especificado");

        RuleForEach(x => x.InfoCuotas!)
            .SetValidator(new InfoCuotaValidator())
            .When(x => x.InfoCuotas != null);
    }
}