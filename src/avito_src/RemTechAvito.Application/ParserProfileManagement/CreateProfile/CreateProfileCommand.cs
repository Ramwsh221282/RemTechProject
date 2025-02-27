using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Injections;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.ParserProfileManagement.CreateProfile;

public sealed record CreateProfileCommand(ProfileNameDto Name)
    : IAvitoCommand<ParserProfileResponse>;

public sealed record ProfileNameDto(string Name)
{
    public ParserProfileName ToValueObject()
    {
        return ParserProfileName.Create(Name);
    }
}

internal sealed class CreateProfileCommandHandler(
    IParserProfileCommandRepository repository,
    ILogger logger,
    ProfileNameDtoValidator validator
) : IAvitoCommandHandler<CreateProfileCommand, ParserProfileResponse>
{
    public async Task<Result<ParserProfileResponse>> Handle(
        CreateProfileCommand command,
        CancellationToken ct = default
    )
    {
        var validation = await validator.ValidateAsync(command.Name, ct);
        if (!validation.IsValid)
            return validation.LogAndReturnError(logger, nameof(CreateProfileCommand));

        var name = command.Name.ToValueObject();
        var profile = new ParserProfile(name);
        var insertion = await repository.Add(profile, ct);
        if (insertion.IsFailure)
            return insertion.Error.LogAndReturnError(logger, nameof(CreateProfileCommand));

        return new ParserProfileResponse(
            profile.Id.Id,
            profile.CreatedOn.Date,
            profile.State.IsActive,
            profile.State.Description,
            profile
                .Links.Select(l => new ParserProfileLinksResponse(
                    l.Id.Id,
                    l.Body.Link,
                    l.Body.Mark
                ))
                .ToArray()
        );
    }
}
