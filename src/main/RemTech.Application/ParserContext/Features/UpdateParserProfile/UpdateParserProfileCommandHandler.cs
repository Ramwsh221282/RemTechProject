using RemTech.Application.ParserContext.Contracts;
using RemTech.Application.ParserContext.Dtos;
using RemTech.Domain.ParserContext;
using RemTech.Domain.ParserContext.Entities.ParserProfiles;
using RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;

namespace RemTech.Application.ParserContext.Features.UpdateParserProfile;

public sealed class UpdateParserProfileCommandHandler(IParserWriteRepository repository)
    : ICommandHandler<UpdateParserProfileCommand, UnitResult<Guid>>
{
    private readonly IParserWriteRepository _repository = repository;

    public async Task<UnitResult<Guid>> Handle(
        UpdateParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        Option<Parser> parser = await _repository.GetByName(command.ParserName, ct);
        if (parser.HasValue == false)
            return UnitResult<Guid>.FromFailure(
                new Error($"Не найден парсер с именем: {command.ParserName}"),
                UnitResultCodes.NotFound
            );

        Option<ParserProfile> profile = parser.Value.GetProfileByName(command.ProfileName);
        if (profile.HasValue == false)
            return UnitResult<Guid>.FromFailure(
                new Error($"Не найден профиль парсера с именем: {command.ProfileName}"),
                UnitResultCodes.NotFound
            );

        UpdateParserProfileDto data = command.Data;
        ParserProfileName? name = ToParserProfileName(data.ProfileName, profile.Value);
        ParserProfileState? state = ToParserProfileState(data.ProfileState);
        ParserProfileSchedule? schedule = ToParserProfileSchedule(data.Schedule);
        ParserProfileLinksCollection? links = ToParserProfileLinks(data.Links);

        profile.Value.Update(name, schedule, state, links);
        await _repository.Save(parser.Value, ct);
        return profile.Value.Id.Value;
    }

    private static ParserProfileName? ToParserProfileName(string? name, ParserProfile current)
    {
        if (name == null)
            return null;
        if (current.Name.Value == name)
            return null;
        return ParserProfileName.Create(name);
    }

    private static ParserProfileState? ToParserProfileState(string? state)
    {
        if (state == null)
            return null;
        return ParserProfileState.Create(state);
    }

    private static ParserProfileSchedule? ToParserProfileSchedule(ParserScheduleDto? dto)
    {
        if (dto?.RepeatEveryHours == null)
            return null;
        return ParserProfileSchedule.CreateFromHour(dto.RepeatEveryHours.Value);
    }

    private static ParserProfileLinksCollection? ToParserProfileLinks(string[]? links)
    {
        if (links == null)
            return null;
        ParserProfileLink[] domainLinks = [.. links.Select(ParserProfileLink.Create)];
        return ParserProfileLinksCollection.Create(domainLinks);
    }
}
