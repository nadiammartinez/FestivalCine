using System.Data;
using Microsoft.Data.SqlClient;

namespace FestivalCine.Database;

public sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("FestCineDb")
            ?? throw new InvalidOperationException("No se encontro la cadena de conexion 'FestCineDb'.");
    }

    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}
