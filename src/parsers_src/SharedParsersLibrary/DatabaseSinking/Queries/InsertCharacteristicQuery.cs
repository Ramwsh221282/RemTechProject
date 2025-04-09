using System.Data;
using Dapper;
using RemTech.Application.AdvertisementsContext.Models.CharacteristicsManagement;
using RemTech.Domain.AdvertisementsContext.ValueObjects;
using RemTech.Infrastructure.PostgreSql.Configuration;

namespace SharedParsersLibrary.DatabaseSinking.Queries;

public sealed class InsertCharacteristicQuery(ConnectionStringFactory factory)
{
    private const string Sql = """
        INSERT INTO characteristics (id, name, value)
        VALUES (@id, @name, @value)
        """;
    private readonly ConnectionStringFactory _factory = factory;

    public async Task Insert(AdvertisementCharacteristic characteristic)
    {
        TransportCharacteristic ctx = new(characteristic);
        Guid id = ctx.Id;
        string name = ctx.Name;
        string value = ctx.Value;
        using IDbConnection connection = _factory.Create();
        await connection.ExecuteAsync(
            Sql,
            new
            {
                id = id,
                name = name,
                value = value,
            }
        );
    }
}
