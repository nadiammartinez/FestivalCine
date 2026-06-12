using Dapper;
using FestivalCine.Database;
using FestivalCine.DTOs.Views;

namespace FestivalCine.Services;

public sealed class ReportesService : IReportesService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ReportesService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<OcupacionPeliculaDto>> ObtenerOcupacionPeliculasAsync()
    {
        const string sql = """
            SELECT *
            FROM dbo.vw_OcupacionPeliculas
            ORDER BY Anio DESC, PorcentajeOcupacion DESC;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<OcupacionPeliculaDto>(sql);
        return result.AsList();
    }

    public async Task<IReadOnlyList<PremioResultadoDto>> ObtenerPremiosAsync()
    {
        const string sql = """
            SELECT *
            FROM dbo.vw_PremiosResultados
            ORDER BY Categoria, PeliculaGanadora;
            """;

        using var connection = _connectionFactory.CreateConnection();
        var result = await connection.QueryAsync<PremioResultadoDto>(sql);
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
}
