namespace SifenApi.Application.DTOs;

public class UsuarioDto
{
    public int DocumentoTipo { get; set; }
    public string DocumentoNumero { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Cargo { get; set; }
}