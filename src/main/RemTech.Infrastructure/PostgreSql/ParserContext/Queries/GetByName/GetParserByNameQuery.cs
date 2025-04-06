using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.ResponseModels;
using RemTech.Shared.SDK.CqrsPattern.Queries;
using RemTech.Shared.SDK.OptionPattern;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.Queries.GetByName;

public sealed record GetParserByNameQuery(string ParserName) : IQuery<Option<ParserResponse>>;
