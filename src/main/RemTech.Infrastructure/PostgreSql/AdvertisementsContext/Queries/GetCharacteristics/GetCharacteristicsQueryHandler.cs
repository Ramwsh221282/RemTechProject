using System.Data;
using Dapper;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;
using RemTech.Infrastructure.PostgreSql.Configuration;
using RemTech.Shared.SDK.CqrsPattern.Queries;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetCharacteristics;

public sealed class GetCharacteristicsQueryHandler(ConnectionStringFactory factory)
    : IQueryHandler<GetCharacteristicsQuery, TransportCharacteristicDao[]>
{
    private const string Sql = "SELECT * FROM characteristics";
    private readonly ConnectionStringFactory _factory = factory;

    public async Task<TransportCharacteristicDao[]> Handle(
        GetCharacteristicsQuery query,
        CancellationToken ct = default
    )
    {
        CommandDefinition command = new(Sql);
        using IDbConnection connection = _factory.Create();

        IEnumerable<TransportCharacteristicDao> data =
            await connection.QueryAsync<TransportCharacteristicDao>(command);

        return [.. data];
    }
}
