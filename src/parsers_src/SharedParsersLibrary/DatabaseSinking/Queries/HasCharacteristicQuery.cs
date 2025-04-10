using System.Data;
using Dapper;
using RemTech.Domain.AdvertisementsContext.ValueObjects;
using RemTech.Infrastructure.PostgreSql.Configuration;

namespace SharedParsersLibrary.DatabaseSinking.Queries;

public sealed class HasCharacteristicQuery(ConnectionStringFactory factory)
{
    private const string Sql = "SELECT COUNT(*) FROM characteristics WHERE name = @name";
    private readonly ConnectionStringFactory _factory = factory;

    public async Task<bool> Has(AdvertisementCharacteristic characteristic)
    {
        string name = characteristic.Name;
        using IDbConnection connection = _factory.Create();
        int count = await connection.ExecuteScalarAsync<int>(Sql, new { name = name });
        return count != 0;
    }
}
