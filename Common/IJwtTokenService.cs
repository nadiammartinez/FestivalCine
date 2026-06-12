using FestivalCine.DTOs.Responses;

namespace FestivalCine.Common;

public interface IJwtTokenService
{
    LoginResponse CreateToken(UsuarioAutenticadoDto usuario);
}
