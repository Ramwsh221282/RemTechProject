using System.Text.Json;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.DaoModels;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.ResponseModels;

public sealed record ParserProfileResponse(
    Guid Id,
    string Name,
    string State,
    int RepeatEveryHours,
    DateTime NextRun,
    string[] Links
)
{
    public static ParserProfileResponse Create(ParserProfileDao dao)
    {
        Guid id = dao.Id;
        string Name = dao.Name;
        string State = dao.State;
        int repeatEveryHours = (int)(dao.RepeatEverySeconds / 3600);
        DateTimeOffset nextRunOffset = DateTimeOffset.FromUnixTimeSeconds(dao.NextRunUnixSeconds);
        DateTime nextRun = nextRunOffset.UtcDateTime;
        string[] links = InitializeLinksFromJson(dao.Links);
        return new ParserProfileResponse(id, Name, State, repeatEveryHours, nextRun, links);
    }

    private static string[] InitializeLinksFromJson(string linksJson)
    {
        using JsonDocument document = JsonDocument.Parse(linksJson);
        JsonElement linkArray = document.RootElement.GetProperty("Links");
        string[] links =
        [
            .. linkArray.EnumerateArray().Select(item => item.GetProperty("Link").GetString()!),
        ];
        return links.ToArray();
    }
}
