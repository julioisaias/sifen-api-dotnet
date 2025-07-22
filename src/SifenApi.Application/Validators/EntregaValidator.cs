using FluentValidation;
using SifenApi.Application.DTOs;

namespace SifenApi.Application.Validators;

public class EntregaValidator : AbstractValidator<EntregaDto>
{
    public EntregaValidator()
    {
        RuleFor(x => x.Tipo)
            .IsInEnum()
            .WithMessage("Tipo de entrega inválido");

        RuleFor(x => x.Monto)
            .GreaterThan(0)
            .WithMessage("El monto debe ser mayor a cero");

        RuleFor(x => x.Moneda)
            .NotEmpty()
            .Length(3)
            .WithMessage("La moneda debe tener 3 caracteres");

        When(x => x.Tipo == 3, () => // Tarjeta
        {
            RuleFor(x => x.InfoTarjeta)
                .NotNull()
                .WithMessage("Información de tarjeta es requerida");

            RuleFor(x => x.InfoTarjeta!.CodigoAutorizacion)
                .NotEmpty()
                .When(x => x.InfoTarjeta != null)
                .WithMessage("Código de autorización es requerido");
        });

        When(x => x.Tipo == 2, () => // Cheque
        {
            RuleFor(x => x.InfoCheque)
                .NotNull()
                .WithMessage("Información de cheque es requerida");

            RuleFor(x => x.InfoCheque!.NumeroCheque)
                .NotEmpty()
                .When(x => x.InfoCheque != null)
                .WithMessage("Número de cheque es requerido");

            RuleFor(x => x.InfoCheque!.Banco)
                .NotEmpty()
                .When(x => x.InfoCheque != null)
                .WithMessage("Banco es requerido");
        });
    }
}