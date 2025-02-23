using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.ParserProfileManagement.CreateProfile;

public sealed record CreateProfileCommand : IAvitoCommand<ParserProfileResponse>;

internal sealed class CreateProfileCommandHandler(
    IParserProfileCommandRepository repository,
    ILogger logger
) : IAvitoCommandHandler<CreateProfileCommand, ParserProfileResponse>
{
    public async Task<Result<ParserProfileResponse>> Handle(
        CreateProfileCommand command,
        CancellationToken ct = default
    )
    {
        logger.Information("{Command} command request", nameof(CreateProfileCommand));
        ParserProfile profile = new ParserProfile();
        Result<Guid> insertion = await repository.Add(profile, ct);

        if (insertion.IsFailure)
            return insertion.Error;

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
