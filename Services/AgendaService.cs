using System.Data;
using Dapper;
using FestivalCine.Database;
using FestivalCine.DTOs.Requests;
using FestivalCine.DTOs.Responses;
using FestivalCine.DTOs.Views;

namespace FestivalCine.Services;

public sealed class AgendaService : IAgendaService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AgendaService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<PeliculaCatalogoDto>> ListarPeliculasAsync()
    {
        const string sql = """
            SELECT *
            FROM dbo.vw_PeliculasCatalogo
            ORDER BY Titulo;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<PeliculaCatalogoDto>(sql);
        return result.AsList();
    }

    public async Task<IReadOnlyList<SalaCatalogoDto>> ListarSalasAsync()
    {
        const string sql = """
            SELECT *
            FROM dbo.vw_SalasCatalogo
            ORDER BY Sede, Sala;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<SalaCatalogoDto>(sql);
        return result.AsList();
    }

    public async Task<IReadOnlyList<EdicionFestivalDto>> ListarEdicionesAsync()
    {
        const string sql = """
            SELECT *
            FROM dbo.vw_EdicionesFestival
            ORDER BY Anio DESC, FechaInicio DESC;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<EdicionFestivalDto>(sql);
        return result.AsList();
    }

    public async Task<IReadOnlyList<ProyeccionCarteleraDto>> ListarProyeccionesAsync()
    {
        const string sql = """
            SELECT *
            FROM dbo.vw_CarteleraProyecciones
            ORDER BY Fecha, Hora, Sala;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<ProyeccionCarteleraDto>(sql);
        return result.AsList();
    }

    public async Task<ProgramarProyeccionResponse> ProgramarProyeccionAsync(ProgramarProyeccionRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        // El procedimiento inserta y el trigger TR_ControlAgenda valida cruces de horario.
        return await connection.QuerySingleAsync<ProgramarProyeccionResponse>(
            "dbo.ProgramarProyeccion",
            new
            {
                request.IdPelicula,
                request.IdSala,
                request.IdEdicion,
                Fecha = request.Fecha.Date,
                request.Hora
            },
            commandType: CommandType.StoredProcedure);
    }
}
