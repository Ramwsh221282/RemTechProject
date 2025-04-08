using System.Data;
using Dapper;
using RemTech.Infrastructure.PostgreSql.Configuration;
using RemTech.Infrastructure.PostgreSql.ParserContext.DaoModels;
using RemTech.Shared.SDK.CqrsPattern.Queries;
using RemTech.Shared.SDK.OptionPattern;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.Queries.GetByName;

public sealed class GetParserByNameQueryHandler(ConnectionStringFactory factory)
    : IQueryHandler<GetParserByNameQuery, Option<ParserDao>>
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
            WHERE 
                p.name = @name
        """;

    public async Task<Option<ParserDao>> Handle(
        GetParserByNameQuery query,
        CancellationToken ct = default
    )
    {
        Dictionary<Guid, ParserDao> daos = [];
        using IDbConnection connection = factory.Create();
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
            splitOn: "id",
            param: new { name = query.ParserName }
        );

        return daos.Count == 0
            ? Option<ParserDao>.None()
            : Option<ParserDao>.Some(daos.First().Value);
    }
}
