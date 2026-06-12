using FestivalCine.Common;
using FestivalCine.DTOs.Requests;
using FestivalCine.DTOs.Responses;
using FestivalCine.DTOs.Views;
using FestivalCine.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FestivalCine.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ISeguridadService _seguridadService;

    public AuthController(ISeguridadService seguridadService)
    {
        _seguridadService = seguridadService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginRequest request)
    {
        var login = await _seguridadService.LoginAsync(request);
        return Ok(ApiResponse<LoginResponse>.Success(login, "Inicio de sesion correcto"));
    }

    [AllowAnonymous]
    [HttpPost("configurar-admin-inicial")]
    [ProducesResponseType(typeof(ApiResponse<OperacionUsuarioSistemaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<OperacionUsuarioSistemaResponse>>> ConfigurarAdminInicial(
        ConfigurarAdminInicialRequest request)
    {
        var operacion = await _seguridadService.ConfigurarAdminInicialAsync(request);
        return Ok(ApiResponse<OperacionUsuarioSistemaResponse>.Success(operacion, operacion.Mensaje));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("usuarios")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<UsuarioSistemaDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<UsuarioSistemaDto>>>> ListarUsuarios()
    {
        var usuarios = await _seguridadService.ListarUsuariosAsync();
        return Ok(ApiResponse<IReadOnlyList<UsuarioSistemaDto>>.Success(usuarios));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("roles")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<RolSistemaDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<RolSistemaDto>>>> ListarRoles()
    {
        var roles = await _seguridadService.ListarRolesAsync();
        return Ok(ApiResponse<IReadOnlyList<RolSistemaDto>>.Success(roles));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("usuarios")]
    [ProducesResponseType(typeof(ApiResponse<RegistrarUsuarioSistemaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<RegistrarUsuarioSistemaResponse>>> RegistrarUsuario(
        RegistrarUsuarioSistemaRequest request)
    {
        var usuario = await _seguridadService.RegistrarUsuarioAsync(request);
        return Ok(ApiResponse<RegistrarUsuarioSistemaResponse>.Success(usuario, usuario.Mensaje));
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("usuarios/{idUsuario}/estado")]
    [ProducesResponseType(typeof(ApiResponse<OperacionUsuarioSistemaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<OperacionUsuarioSistemaResponse>>> CambiarEstadoUsuario(
        string idUsuario,
        CambiarEstadoUsuarioRequest request)
    {
        var operacion = await _seguridadService.CambiarEstadoUsuarioAsync(idUsuario, request);
        return Ok(ApiResponse<OperacionUsuarioSistemaResponse>.Success(operacion, operacion.Mensaje));
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("usuarios/{idUsuario}/password")]
    [ProducesResponseType(typeof(ApiResponse<OperacionUsuarioSistemaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<OperacionUsuarioSistemaResponse>>> CambiarPasswordUsuario(
        string idUsuario,
        CambiarPasswordUsuarioRequest request)
    {
        var operacion = await _seguridadService.CambiarPasswordUsuarioAsync(idUsuario, request);
        return Ok(ApiResponse<OperacionUsuarioSistemaResponse>.Success(operacion, operacion.Mensaje));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("usuarios/{idUsuario}/roles")]
    [ProducesResponseType(typeof(ApiResponse<OperacionUsuarioSistemaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<OperacionUsuarioSistemaResponse>>> AsignarRolUsuario(
        string idUsuario,
        AsignarRolUsuarioRequest request)
    {
        var operacion = await _seguridadService.AsignarRolUsuarioAsync(idUsuario, request);
        return Ok(ApiResponse<OperacionUsuarioSistemaResponse>.Success(operacion, operacion.Mensaje));
    }
}
