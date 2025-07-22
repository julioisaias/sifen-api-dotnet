using Ardalis.GuardClauses;

namespace SifenApi.Domain.ValueObjects;

public class Cdc : ValueObject
{
    public string Value { get; private set; } = string.Empty;

    // Parameterless constructor for EF Core
    private Cdc() { }

    public Cdc(string value)
    {
        Guard.Against.NullOrWhiteSpace(value, nameof(value));
        Guard.Against.InvalidInput(value, nameof(value), v => v.Length == 44, 
            "CDC debe tener exactamente 44 caracteres");
        
        SetValue(value);
    }

    public static Cdc Generate(
        string tipoDocumento,
        string rucEmisor,
        string digitoVerificadorRuc,
        string establecimiento,
        string puntoExpedicion,
        string numeroDocumento,
        string tipoContribuyente,
        DateTime fechaEmision,
        string tipoEmision,
        string codigoSeguridad)
    {
        var fechaStr = fechaEmision.ToString("yyyyMMdd");
        
        var cdcBase = $"{tipoDocumento.PadLeft(2, '0')}" +
                      $"{rucEmisor.PadLeft(8, '0')}" +
                      $"{digitoVerificadorRuc}" +
                      $"{establecimiento.PadLeft(3, '0')}" +
                      $"{puntoExpedicion.PadLeft(3, '0')}" +
                      $"{numeroDocumento.PadLeft(7, '0')}" +
                      $"{tipoContribuyente}" +
                      $"{fechaStr}" +
                      $"{tipoEmision}" +
                      $"{codigoSeguridad.PadLeft(9, '0')}";

        var digitoVerificador = CalcularDigitoVerificador(cdcBase);
        
        return new Cdc($"{cdcBase}{digitoVerificador}");
    }

    private static string CalcularDigitoVerificador(string cdcBase)
    {
        // Implementación del algoritmo de dígito verificador según manual técnico SIFEN
        var baseMax = 11;
        var total = 0;
        var k = 2;

        for (var i = cdcBase.Length - 1; i >= 0; i--)
        {
            total += int.Parse(cdcBase[i].ToString()) * k;
            k++;
            if (k > baseMax) k = 2;
        }

        var resto = total % baseMax;
        var digitoVerificador = resto > 1 ? baseMax - resto : 0;

        return digitoVerificador.ToString();
    }

    private void SetValue(string value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}