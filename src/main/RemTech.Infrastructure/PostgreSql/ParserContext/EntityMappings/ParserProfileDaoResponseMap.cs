using Dapper.FluentMap.Mapping;
using RemTech.Infrastructure.PostgreSql.ParserContext.DaoModels;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.EntityMappings;

public sealed class ParserProfileDaoResponseMap : EntityMap<ParserProfileDao>
{
    public ParserProfileDaoResponseMap()
    {
        Map(i => i.Id).ToColumn("id");
        Map(p => p.Name).ToColumn("name");
        Map(p => p.State).ToColumn("state");
        Map(p => p.RepeatEverySeconds).ToColumn("repeat_every_seconds");
        Map(p => p.NextRunUnixSeconds).ToColumn("next_run_unix_seconds");
        Map(p => p.Links).ToColumn("Links");
        Map(p => p.ParserId).ToColumn("parser_id");
    }
}
