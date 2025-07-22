using FluentValidation;
using SifenApi.Application.DTOs;

namespace SifenApi.Application.Validators;

public class CondicionValidator : AbstractValidator<CondicionDto>
{
    public CondicionValidator()
    {
        RuleFor(x => x.Tipo)
            .IsInEnum()
            .WithMessage("Tipo de condición de venta inválido");

        When(x => x.Tipo == 1, () => // Contado
        {
            RuleFor(x => x.Entregas)
                .NotEmpty()
                .WithMessage("Debe especificar al menos una forma de pago para venta al contado");

            RuleForEach(x => x.Entregas!)
                .SetValidator(new EntregaValidator());
        });

        When(x => x.Tipo == 2, () => // Crédito
        {
            RuleFor(x => x.Credito)
                .NotNull()
                .WithMessage("Debe especificar los datos del crédito");

            RuleFor(x => x.Credito!)
                .SetValidator(new CreditoValidator())
                .When(x => x.Credito != null);
        });
    }
}