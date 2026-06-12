namespace FestivalCine.DTOs.Requests;

public sealed class ConfigurarAdminInicialRequest
{
    public required string NombreUsuario { get; init; }
    public required string Password { get; init; }
}
