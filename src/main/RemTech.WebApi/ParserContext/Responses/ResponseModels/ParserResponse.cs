using RemTech.Infrastructure.PostgreSql.ParserContext.DaoModels;
using RemTech.WebApi.ParserContext.Responses.ResponseModels;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.ResponseModels;

public sealed record ParserResponse(Guid Id, string Name, ParserProfileResponse[] Profiles)
{
    public static ParserResponse Create(ParserDao dao)
    {
        Guid id = dao.Id;
        string name = dao.Name;
        ParserProfileResponse[] profiles = [.. dao.Profiles.Select(ParserProfileResponse.Create)];
        return new ParserResponse(id, name, profiles);
    }
}
