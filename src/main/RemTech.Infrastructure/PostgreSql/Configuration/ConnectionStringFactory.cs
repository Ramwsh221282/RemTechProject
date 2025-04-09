using System.Data;
using Npgsql;

namespace RemTech.Infrastructure.PostgreSql.Configuration;

public sealed class ConnectionStringFactory(ConnectionString connectionString)
{
    private readonly ConnectionString _connectionString = connectionString;

    public IDbConnection Create()
    {
        IDbConnection connection = new NpgsqlConnection(_connectionString.Value);
        connection.Open();
        return connection;
    }
}
