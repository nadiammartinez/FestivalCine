using FestivalCine.DTOs.Requests;
using FestivalCine.DTOs.Responses;
using FestivalCine.DTOs.Views;

namespace FestivalCine.Services;

public interface ISeguridadService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<IReadOnlyList<UsuarioSistemaDto>> ListarUsuariosAsync();
    Task<IReadOnlyList<RolSistemaDto>> ListarRolesAsync();
    Task<OperacionUsuarioSistemaResponse> ConfigurarAdminInicialAsync(ConfigurarAdminInicialRequest request);
    Task<RegistrarUsuarioSistemaResponse> RegistrarUsuarioAsync(RegistrarUsuarioSistemaRequest request);
    Task<OperacionUsuarioSistemaResponse> CambiarEstadoUsuarioAsync(string idUsuario, CambiarEstadoUsuarioRequest request);
    Task<OperacionUsuarioSistemaResponse> CambiarPasswordUsuarioAsync(string idUsuario, CambiarPasswordUsuarioRequest request);
    Task<OperacionUsuarioSistemaResponse> AsignarRolUsuarioAsync(string idUsuario, AsignarRolUsuarioRequest request);
}
