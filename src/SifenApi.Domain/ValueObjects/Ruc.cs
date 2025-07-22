using Ardalis.GuardClauses;
using System.Text.RegularExpressions;

namespace SifenApi.Domain.ValueObjects;

public class Ruc : ValueObject
{
    public string Numero { get; private set; } = string.Empty;
    public string DigitoVerificador { get; private set; } = string.Empty;
    public string Completo => $"{Numero}-{DigitoVerificador}";

    // Parameterless constructor for EF Core
    private Ruc() { }

    public Ruc(string rucCompleto)
    {
        Guard.Against.NullOrWhiteSpace(rucCompleto, nameof(rucCompleto));
        
        var pattern = @"^(\d{1,8})-(\d)$";
        var match = Regex.Match(rucCompleto, pattern);
        
        if (!match.Success)
            throw new ArgumentException("Formato de RUC inválido. Debe ser XXXXXXXX-X");

        SetNumero(match.Groups[1].Value.PadLeft(8, '0'));
        SetDigitoVerificador(match.Groups[2].Value);
        
        if (!ValidarDigitoVerificador())
            throw new ArgumentException("Dígito verificador del RUC inválido");
    }

    private bool ValidarDigitoVerificador()
    {
        var baseMax = 11;
        var k = 2;
        var total = 0;
        
        for (var i = Numero.Length - 1; i >= 0; i--)
        {
            total += int.Parse(Numero[i].ToString()) * k;
            k++;
            if (k > baseMax) k = 2;
        }

        var resto = total % baseMax;
        var digitoCalculado = resto > 1 ? baseMax - resto : 0;
        
        return digitoCalculado.ToString() == DigitoVerificador;
    }

    private void SetNumero(string numero)
    {
        Numero = numero;
    }

    private void SetDigitoVerificador(string digitoVerificador)
    {
        DigitoVerificador = digitoVerificador;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Numero;
        yield return DigitoVerificador;
    }
}
