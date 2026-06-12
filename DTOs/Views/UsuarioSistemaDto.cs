namespace FestivalCine.DTOs.Views;

public sealed class UsuarioSistemaDto
{
    public required string IdUsuario { get; init; }
    public required string NombreUsuario { get; init; }
    public required string NombreCompleto { get; init; }
    public required string Email { get; init; }
    public required string Estado { get; init; }
    public DateTime FechaCreacion { get; init; }
    public DateTime? UltimoAcceso { get; init; }
    public string? Roles { get; init; }
}
