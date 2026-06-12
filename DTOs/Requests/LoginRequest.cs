namespace FestivalCine.DTOs.Requests;

public sealed class LoginRequest
{
    public required string NombreUsuario { get; init; }
    public required string Password { get; init; }
}
