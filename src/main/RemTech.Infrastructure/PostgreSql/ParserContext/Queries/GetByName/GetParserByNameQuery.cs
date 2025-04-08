using RemTech.Shared.SDK.CqrsPattern.Queries;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.Queries.GetByName;

public sealed record GetParserByNameQuery(string ParserName) : IQuery;
