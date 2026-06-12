namespace FestivalCine.DTOs.Responses;

public sealed class LoginResponse
{
    public required string Token { get; init; }
    public DateTime ExpiraEn { get; init; }
    public required UsuarioAutenticadoDto Usuario { get; init; }
}
