using System.Data;
using Dapper;
using RemTech.Domain.AdvertisementsContext;
using RemTech.Infrastructure.PostgreSql.Configuration;

namespace SharedParsersLibrary.DatabaseSinking.Queries;

public sealed class HasAdvertisementQuery(ConnectionStringFactory factory)
{
    private const string Sql = "SELECT COUNT(*) FROM advertisements WHERE id = @id";
    private readonly ConnectionStringFactory _factory = factory;

    public async Task<bool> Has(Advertisement advertisement)
    {
        using IDbConnection connection = _factory.Create();
        int count = await connection.ExecuteScalarAsync<int>(
            Sql,
            new { id = advertisement.Id.Value }
        );
        return count == 0;
    }
}
