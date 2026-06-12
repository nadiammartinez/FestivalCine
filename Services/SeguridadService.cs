using System.Data;
using Dapper;
using FestivalCine.Common;
using FestivalCine.Database;
using FestivalCine.DTOs.Internal;
using FestivalCine.DTOs.Requests;
using FestivalCine.DTOs.Responses;
using FestivalCine.DTOs.Views;

namespace FestivalCine.Services;

public sealed class SeguridadService : ISeguridadService
{
    private const string AdminPasswordPlaceholder = "CAMBIAR_POR_HASH_REAL";

    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public SeguridadService(
        IDbConnectionFactory connectionFactory,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _connectionFactory = connectionFactory;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        var rows = (await connection.QueryAsync<UsuarioLoginRow>(
            "dbo.ObtenerUsuarioParaLogin",
            new { request.NombreUsuario },
            commandType: CommandType.StoredProcedure)).AsList();

        var firstRow = rows.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("Usuario o contrasena incorrectos.");

        if (!_passwordHasher.Verify(request.Password, firstRow.PasswordHash))
            throw new UnauthorizedAccessException("Usuario o contrasena incorrectos.");

        var usuario = new UsuarioAutenticadoDto
        {
            IdUsuario = firstRow.IdUsuario,
            NombreUsuario = firstRow.NombreUsuario,
            NombreCompleto = firstRow.NombreCompleto,
            Email = firstRow.Email,
            Roles = rows.Select(row => row.Rol).Distinct().ToArray()
        };

        await connection.ExecuteAsync(
            "dbo.RegistrarAccesoUsuarioSistema",
            new { firstRow.IdUsuario },
            commandType: CommandType.StoredProcedure);

        return _jwtTokenService.CreateToken(usuario);
    }

    public async Task<IReadOnlyList<UsuarioSistemaDto>> ListarUsuariosAsync()
    {
        const string sql = """
            SELECT *
            FROM dbo.vw_UsuariosSistema
            ORDER BY NombreUsuario;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<UsuarioSistemaDto>(sql);
        return result.AsList();
    }

    public async Task<IReadOnlyList<RolSistemaDto>> ListarRolesAsync()
    {
        const string sql = """
            SELECT *
            FROM dbo.vw_RolesSistema
            ORDER BY Nombre;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<RolSistemaDto>(sql);
        return result.AsList();
    }

    public async Task<OperacionUsuarioSistemaResponse> ConfigurarAdminInicialAsync(ConfigurarAdminInicialRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        var rows = (await connection.QueryAsync<UsuarioLoginRow>(
            "dbo.ObtenerUsuarioParaLogin",
            new { request.NombreUsuario },
            commandType: CommandType.StoredProcedure)).AsList();

        var firstRow = rows.FirstOrDefault()
            ?? throw new UnauthorizedAccessException("No se encontro el usuario administrador inicial.");

        var isAdmin = rows.Any(row => row.Rol == "Admin");
        if (!isAdmin || firstRow.PasswordHash != AdminPasswordPlaceholder)
            throw new UnauthorizedAccessException("El administrador inicial ya fue configurado o no tiene permisos de Admin.");

        return await connection.QuerySingleAsync<OperacionUsuarioSistemaResponse>(
            "dbo.CambiarPasswordUsuarioSistema",
            new
            {
                firstRow.IdUsuario,
                PasswordHash = _passwordHasher.Hash(request.Password)
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<RegistrarUsuarioSistemaResponse> RegistrarUsuarioAsync(RegistrarUsuarioSistemaRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        var passwordHash = _passwordHasher.Hash(request.Password);

        return await connection.QuerySingleAsync<RegistrarUsuarioSistemaResponse>(
            "dbo.RegistrarUsuarioSistema",
            new
            {
                request.NombreUsuario,
                request.NombreCompleto,
                request.Email,
                PasswordHash = passwordHash,
                request.Rol
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<OperacionUsuarioSistemaResponse> CambiarEstadoUsuarioAsync(
        string idUsuario,
        CambiarEstadoUsuarioRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<OperacionUsuarioSistemaResponse>(
            "dbo.CambiarEstadoUsuarioSistema",
            new
            {
                IdUsuario = idUsuario,
                request.Estado
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<OperacionUsuarioSistemaResponse> CambiarPasswordUsuarioAsync(
        string idUsuario,
        CambiarPasswordUsuarioRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<OperacionUsuarioSistemaResponse>(
            "dbo.CambiarPasswordUsuarioSistema",
            new
            {
                IdUsuario = idUsuario,
                PasswordHash = _passwordHasher.Hash(request.Password)
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<OperacionUsuarioSistemaResponse> AsignarRolUsuarioAsync(
        string idUsuario,
        AsignarRolUsuarioRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<OperacionUsuarioSistemaResponse>(
            "dbo.AsignarRolUsuarioSistema",
            new
            {
                IdUsuario = idUsuario,
                request.Rol
            },
            commandType: CommandType.StoredProcedure);
    }
}
