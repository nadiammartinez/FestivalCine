namespace FestivalCine.DTOs.Internal;

internal sealed class UsuarioLoginRow
{
    public required string IdUsuario { get; init; }
    public required string NombreUsuario { get; init; }
    public required string NombreCompleto { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
    public required string Estado { get; init; }
    public required string Rol { get; init; }
}
