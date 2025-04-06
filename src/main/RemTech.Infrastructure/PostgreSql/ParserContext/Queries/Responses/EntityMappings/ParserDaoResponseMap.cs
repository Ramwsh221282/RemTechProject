using Dapper.FluentMap.Mapping;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.DaoModels;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.EntityMappings;

public sealed class ParserDaoResponseMap : EntityMap<ParserDao>
{
    public ParserDaoResponseMap()
    {
        Map(i => i.Id).ToColumn("id");
        Map(i => i.Name).ToColumn("name");
    }
}
