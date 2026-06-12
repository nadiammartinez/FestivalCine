namespace FestivalCine.DTOs.Responses;

public sealed class RegistrarUsuarioSistemaResponse
{
    public required string Mensaje { get; init; }
    public required string IdUsuarioGenerado { get; init; }
}
