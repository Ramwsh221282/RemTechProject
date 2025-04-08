using Dapper.FluentMap.Mapping;
using RemTech.Infrastructure.PostgreSql.ParserContext.DaoModels;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.EntityMappings;

public sealed class ParserDaoResponseMap : EntityMap<ParserDao>
{
    public ParserDaoResponseMap()
    {
        Map(i => i.Id).ToColumn("id");
        Map(i => i.Name).ToColumn("name");
    }
}
