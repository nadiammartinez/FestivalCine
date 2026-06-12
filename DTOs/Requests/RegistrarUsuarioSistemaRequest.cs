namespace FestivalCine.DTOs.Requests;

public sealed class RegistrarUsuarioSistemaRequest
{
    public required string NombreUsuario { get; init; }
    public required string NombreCompleto { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Rol { get; init; }
}
