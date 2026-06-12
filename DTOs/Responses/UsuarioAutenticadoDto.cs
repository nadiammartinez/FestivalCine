namespace FestivalCine.DTOs.Responses;

public sealed class UsuarioAutenticadoDto
{
    public required string IdUsuario { get; init; }
    public required string NombreUsuario { get; init; }
    public required string NombreCompleto { get; init; }
    public required string Email { get; init; }
    public required IReadOnlyList<string> Roles { get; init; }
}
