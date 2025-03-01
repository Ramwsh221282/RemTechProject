using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Injections;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.ParserProfileManagement.UpdateParserProfileLinks;

public sealed record ParserProfileDto(
    string Id,
    string Name,
    bool State,
    ParserProfileLinkDto[] Links
)
{
    public ParserProfileName GetName()
    {
        return ParserProfileName.Create(Name);
    }

    public ParserProfileState GetState()
    {
        return State ? ParserProfileState.CreateActive() : ParserProfileState.CreateInactive();
    }

    public ParserProfileLink[] GetLinks()
    {
        return Links
            .Select(l => ParserProfileLinkFactory.Create(l.Mark, l.Link, l.Additions).Value)
            .ToArray();
    }
}

public sealed record ParserProfileLinkDto(string Mark, string Link, List<string>? Additions = null);

public sealed record UpdateParserProfileCommand(ParserProfileDto Dto) : IAvitoCommand
{
    internal static void Register(IServiceCollection services)
    {
        services.AddScoped<
            IAvitoCommandHandler<UpdateParserProfileCommand>,
            UpdateParserProfileCommandHandler
        >();
        services.AddScoped<UpdateParserProfileLinksCommandValidator>();
    }
}

internal sealed class UpdateParserProfileCommandHandler(
    IParserProfileReadRepository readRepository,
    IParserProfileCommandRepository commandRepository,
    UpdateParserProfileLinksCommandValidator validator,
    ILogger logger
) : IAvitoCommandHandler<UpdateParserProfileCommand>
{
    public async Task<Result> Handle(
        UpdateParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.LogAndReturnError(logger, nameof(UpdateParserProfileCommand));

        var profile = await readRepository.GetById(command.Dto.Id, ct);
        if (profile.IsFailure)
            return profile.Error.LogAndReturnError(logger, nameof(UpdateParserProfileCommand));

        var name = command.Dto.GetName();
        var state = command.Dto.GetState();
        var links = command.Dto.GetLinks();
        var updated = profile.Value.Update(name, state, links);
        var update = await commandRepository.Update(updated, ct);
        return update;
    }
}
