using System.Data;
using Dapper;
using RemTech.Infrastructure.PostgreSql.Configuration;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.DaoModels;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.ResponseModels;
using RemTech.Shared.SDK.CqrsPattern.Queries;
using RemTech.Shared.SDK.OptionPattern;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.Queries.GetByName;

public sealed class GetParserByNameQueryHandler(ConnectionStringFactory factory)
    : IQueryHandler<GetParserByNameQuery, Option<ParserResponse>>
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

    public async Task<Option<ParserResponse>> Handle(
        GetParserByNameQuery query,
        CancellationToken ct = default
    )
    {
        using IDbConnection connection = factory.Create();

        ParserDao? parser = await connection.QueryFirstOrDefaultAsync<ParserDao>(
            Sql,
            new { name = query.ParserName }
        );

        return parser == null
            ? Option<ParserResponse>.None()
            : Option<ParserResponse>.Some(ParserResponse.Create(parser));
    }
}
