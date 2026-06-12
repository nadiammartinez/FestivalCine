using System.Data;
using Dapper;
using FestivalCine.Database;
using FestivalCine.DTOs.Requests;
using FestivalCine.DTOs.Responses;
using FestivalCine.DTOs.Views;

namespace FestivalCine.Services;

public sealed class TaquillaService : ITaquillaService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TaquillaService(IDbConnectionFactory connectionFactory)
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

    public async Task<IReadOnlyList<ProyeccionCarteleraDto>> ListarCarteleraAsync()
    {
        const string sql = """
            SELECT *
            FROM dbo.vw_CarteleraProyecciones
            ORDER BY Fecha, Hora, Titulo;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<ProyeccionCarteleraDto>(sql);
        return result.AsList();
    }

    public async Task<IReadOnlyList<ProyeccionCarteleraDto>> ListarProyeccionesDisponiblesAsync(string idPelicula)
    {
        const string sql = """
            SELECT *
            FROM dbo.vw_CarteleraProyecciones
            WHERE IdPelicula = @IdPelicula
              AND AforoDisponible > 0
            ORDER BY Fecha, Hora;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<ProyeccionCarteleraDto>(sql, new { IdPelicula = idPelicula });
        return result.AsList();
    }

    public async Task<IReadOnlyList<EventoParaleloDto>> ListarEventosAsync()
    {
        const string sql = """
            SELECT *
            FROM dbo.vw_EventosParalelosCartelera
            ORDER BY Fecha, Hora, Nombre;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<EventoParaleloDto>(sql);
        return result.AsList();
    }

    public async Task<IReadOnlyList<RecaudacionVentaDto>> ObtenerRecaudacionAsync()
    {
        const string sql = """
            SELECT *
            FROM dbo.vw_RecaudacionVentas
            ORDER BY TipoVenta, TipoTarifa;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<RecaudacionVentaDto>(sql);
        return result.AsList();
    }

    public async Task<CompraEntradaResponse> ComprarEntradaAsync(ComprarEntradaRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        // La validacion de aforo y tarifa vive en SQL Server.
        return await connection.QuerySingleAsync<CompraEntradaResponse>(
            "dbo.ComprarEntrada",
            new
            {
                request.IdAsistente,
                request.IdProyeccion,
                request.TipoEntrada
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<CompraEntradaResponse> ComprarEntradaEventoAsync(ComprarEntradaEventoRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        // La validacion de aforo del evento vive en SQL Server.
        return await connection.QuerySingleAsync<CompraEntradaResponse>(
            "dbo.ComprarEntradaEvento",
            new
            {
                request.IdAsistente,
                request.IdEvento,
                request.TipoEntrada
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<VenderAbonoResponse> VenderAbonoAsync(VenderAbonoRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        // La venta de abono, pago, factura y rollback viven en SQL Server.
        return await connection.QuerySingleAsync<VenderAbonoResponse>(
            "dbo.VenderAbono",
            new
            {
                request.IdAsistente,
                request.TipoAbono,
                request.IdEdicion,
                request.PagoAprobado
            },
            commandType: CommandType.StoredProcedure);
    }
}
