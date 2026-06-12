namespace FestivalCine.DTOs.Responses;

public sealed class OperacionUsuarioSistemaResponse
{
    public required string Mensaje { get; init; }
    public required string IdUsuario { get; init; }
    public string? Estado { get; init; }
    public string? Rol { get; init; }
}
