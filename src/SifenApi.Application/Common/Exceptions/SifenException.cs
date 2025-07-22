namespace SifenApi.Application.Common.Exceptions;

public class SifenException : Exception
{
    public string? Codigo { get; }
    public List<string> Detalles { get; }

    public SifenException(string mensaje, string? codigo = null) : base(mensaje)
    {
        Codigo = codigo;
        Detalles = new List<string>();
    }

    public SifenException(string mensaje, List<string> detalles, string? codigo = null) : base(mensaje)
    {
        Codigo = codigo;
        Detalles = detalles ?? new List<string>();
    }
}
