using System.Data;
using Dapper;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;
using RemTech.Infrastructure.PostgreSql.Configuration;
using RemTech.Shared.SDK.CqrsPattern.Queries;
using RemTech.Shared.SDK.OptionPattern;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisementById;

public sealed class GetAdvertisementByIdQueryHandler(ConnectionStringFactory factory)
    : IQueryHandler<GetAdvertisementByIdQuery, Option<AdvertisementDao>>
{
    private readonly ConnectionStringFactory _factory = factory;
    private const string Sql = "SELECT * FROM advertisements WHERE id = @id";

    public async Task<Option<AdvertisementDao>> Handle(
        GetAdvertisementByIdQuery query,
        CancellationToken ct = default
    )
    {
        var parameters = new { query.Id };
        CommandDefinition command = new(Sql, parameters, cancellationToken: ct);

        using IDbConnection connection = _factory.Create();

        AdvertisementDao? advertisement =
            await connection.QueryFirstOrDefaultAsync<AdvertisementDao>(command);

        return advertisement == null
            ? Option<AdvertisementDao>.None()
            : Option<AdvertisementDao>.Some(advertisement);
    }
}
