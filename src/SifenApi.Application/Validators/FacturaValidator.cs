using FluentValidation;
using SifenApi.Application.DTOs;
using SifenApi.Domain.Enums;

namespace SifenApi.Application.Validators;

public class FacturaValidator : AbstractValidator<FacturaDto>
{
    public FacturaValidator()
    {
        RuleFor(x => x.TipoDocumento)
            .IsInEnum()
            .WithMessage("Tipo de documento inválido");

        RuleFor(x => x.Establecimiento)
            .NotEmpty()
            .Length(3)
            .Matches(@"^\d{3}$")
            .WithMessage("El establecimiento debe ser numérico de 3 dígitos");

        RuleFor(x => x.PuntoExpedicion)
            .NotEmpty()
            .Length(3)
            .Matches(@"^\d{3}$")
            .WithMessage("El punto de expedición debe ser numérico de 3 dígitos");

        RuleFor(x => x.Fecha)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.Now.AddDays(1))
            .WithMessage("La fecha no puede ser futura");

        RuleFor(x => x.TipoEmision)
            .IsInEnum()
            .WithMessage("Tipo de emisión inválido");

        RuleFor(x => x.TipoTransaccion)
            .IsInEnum()
            .WithMessage("Tipo de transacción inválido");

        RuleFor(x => x.Moneda)
            .NotEmpty()
            .Length(3)
            .WithMessage("La moneda debe tener 3 caracteres");

        RuleFor(x => x.Cambio)
            .GreaterThan(0)
            .When(x => x.Moneda != "PYG")
            .WithMessage("El tipo de cambio es requerido para moneda extranjera");

        RuleFor(x => x.Cliente)
            .NotNull()
            .SetValidator(new ClienteValidator());

        RuleFor(x => x.Condicion)
            .NotNull()
            .SetValidator(new CondicionValidator());

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("Debe incluir al menos un item");

        RuleForEach(x => x.Items)
            .SetValidator(new ItemValidator());

        // Validaciones específicas por tipo de documento
        When(x => x.TipoDocumento == (int)TipoDocumento.NotaCreditoElectronica || 
                  x.TipoDocumento == (int)TipoDocumento.NotaDebitoElectronica, () =>
        {
            RuleFor(x => x.DocumentoAsociado)
                .NotNull()
                .WithMessage("Documento asociado es requerido para notas de crédito/débito");
        });

        When(x => x.TipoDocumento == (int)TipoDocumento.NotaRemisionElectronica, () =>
        {
            RuleFor(x => x.DetalleTransporte)
                .NotNull()
                .WithMessage("Detalle de transporte es requerido para nota de remisión");
        });
    }
}