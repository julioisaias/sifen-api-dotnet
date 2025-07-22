using FluentValidation;
using SifenApi.Application.DTOs;

namespace SifenApi.Application.Validators;

public class ClienteValidator : AbstractValidator<ClienteDto>
{
    public ClienteValidator()
    {
        When(x => x.Contribuyente, () =>
        {
            RuleFor(x => x.Ruc)
                .NotEmpty()
                .Matches(@"^\d{1,8}-\d$")
                .WithMessage("RUC inválido. Formato: XXXXXXXX-X");

            RuleFor(x => x.RazonSocial)
                .NotEmpty()
                .MaximumLength(255)
                .WithMessage("Razón social es requerida para contribuyentes");

            RuleFor(x => x.TipoContribuyente)
                .NotNull()
                .IsInEnum()
                .WithMessage("Tipo de contribuyente es requerido");
        });

        When(x => !x.Contribuyente, () =>
        {
            RuleFor(x => x.DocumentoTipo)
                .NotNull()
                .IsInEnum()
                .WithMessage("Tipo de documento es requerido para no contribuyentes");

            RuleFor(x => x.DocumentoNumero)
                .NotEmpty()
                .MaximumLength(20)
                .WithMessage("Número de documento es requerido para no contribuyentes");

            RuleFor(x => x.RazonSocial)
                .NotEmpty()
                .When(x => string.IsNullOrEmpty(x.NombreFantasia))
                .WithMessage("Debe especificar razón social o nombre de fantasía");
        });

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email inválido");

        RuleFor(x => x.Telefono)
            .Matches(@"^[\d\-\+\(\)\s]+$")
            .When(x => !string.IsNullOrEmpty(x.Telefono))
            .WithMessage("Teléfono contiene caracteres inválidos");

        RuleFor(x => x.Pais)
            .Length(3)
            .When(x => !string.IsNullOrEmpty(x.Pais))
            .WithMessage("El código de país debe tener 3 caracteres");
    }
}