using System.Data;
using Dapper;
using RemTech.Infrastructure.PostgreSql.Configuration;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.DaoModels;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.ResponseModels;
using RemTech.Shared.SDK.CqrsPattern.Queries;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.Queries.GetAllParsers;

public sealed class GetAllParsersQueryHandler(ConnectionStringFactory factory)
    : IQueryHandler<GetAllParsersQuery, ParserResponse[]>
{
    private const string Sql = """
            SELECT 
            p.id, 
            p.name, 
            pr.id, 
            pr.name, 
            pr.state, 
            pr.next_run_unix_seconds, 
            pr.repeat_every_seconds, 
            pr."Links",
            pr.parser_id
        FROM 
            parsers p
        LEFT JOIN 
            parser_profiles pr ON p.id = pr.parser_id
        """;

    private readonly ConnectionStringFactory _factory = factory;

    public async Task<ParserResponse[]> Handle(
        GetAllParsersQuery query,
        CancellationToken ct = default
    )
    {
        Dictionary<Guid, ParserDao> daos = [];
        using IDbConnection connection = _factory.Create();
        await connection.QueryAsync<ParserDao, ParserProfileDao?, ParserDao>(
            Sql,
            (parser, profile) =>
            {
                if (!daos.TryGetValue(parser.Id, out ParserDao? dao))
                {
                    dao = new ParserDao()
                    {
                        Id = parser.Id,
                        Name = parser.Name,
                        Profiles = [],
                    };
                    daos.Add(dao.Id, dao);
                }

                if (profile != null && profile.Id != Guid.Empty)
                {
                    profile.ParserId = parser.Id;
                    dao.Profiles.Add(profile);
                }

                return parser;
            },
            splitOn: "id"
        );
        return daos.Count == 0 ? [] : [.. daos.Values.Select(ParserResponse.Create)];
    }
}
