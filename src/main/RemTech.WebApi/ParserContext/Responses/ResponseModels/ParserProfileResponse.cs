using RemTech.Domain.ParserContext.Entities.ParserProfiles;
using RemTech.Infrastructure.PostgreSql.ParserContext.DaoModels;

namespace RemTech.WebApi.ParserContext.Responses.ResponseModels;

public sealed record ParserProfileResponse(
    Guid Id,
    string Name,
    string State,
    int RepeatEveryHours,
    DateTime NextRun,
    string[] Links
)
{
    public static ParserProfileResponse Create(ParserProfileDao profileDao)
    {
        Guid id = profileDao.Id;
        string Name = profileDao.Name;
        string State = profileDao.State;
        int repeatEveryHours = (int)(profileDao.RepeatEverySeconds / 3600);
        DateTimeOffset nextRunOffset = DateTimeOffset.FromUnixTimeSeconds(
            profileDao.NextRunUnixSeconds
        );
        DateTime nextRun = nextRunOffset.UtcDateTime;
        string[] links = profileDao.GetDeserializedLinksArray();
        return new ParserProfileResponse(id, Name, State, repeatEveryHours, nextRun, links);
    }

    public static ParserProfileResponse Create(ParserProfile profileDomain)
    {
        Guid id = profileDomain.Id.Value;
        string name = profileDomain.Name.Value;
        string state = profileDomain.State.State;
        int repeatHours = (int)(profileDomain.Schedule.RepeatEveryUnixSeconds / 3600);
        DateTimeOffset nextRunOffset = DateTimeOffset.FromUnixTimeSeconds(
            profileDomain.Schedule.NextRunUnixSeconds
        );
        DateTime nextRun = nextRunOffset.UtcDateTime;
        string[] links = [.. profileDomain.Links.Links.Select(l => l.Link)];
        return new ParserProfileResponse(id, name, state, repeatHours, nextRun, links);
    }
}
