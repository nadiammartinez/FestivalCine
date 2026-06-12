using System.Data;

namespace FestivalCine.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
