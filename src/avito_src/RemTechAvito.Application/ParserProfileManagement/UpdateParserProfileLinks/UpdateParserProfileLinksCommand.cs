using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Contracts.Common.Dto.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks;
using RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks.ValueObjects;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.ParserProfileManagement.UpdateParserProfileLinks;

public sealed record UpdateParserProfileLinksCommand(string ProfileId, ParserProfileLinkDto[] Links)
    : IAvitoCommand;

internal sealed class UpdateParserProfileLinksCommandHandler
    : IAvitoCommandHandler<UpdateParserProfileLinksCommand>
{
    private readonly IParserProfileReadRepository _readRepository;
    private readonly IParserProfileCommandRepository _commandRepository;
    private readonly ILogger _logger;

    public UpdateParserProfileLinksCommandHandler(
        IParserProfileReadRepository readRepository,
        IParserProfileCommandRepository commandRepository,
        ILogger logger
    )
    {
        _readRepository = readRepository;
        _commandRepository = commandRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(
        UpdateParserProfileLinksCommand command,
        CancellationToken ct = default
    )
    {
        _logger.Information(
            "{Command} requested. Body: {Body}",
            nameof(UpdateParserProfileLinksCommand),
            command
        );

        if (command.Links.Length == 0)
        {
            string error = "New parser profile links are empty";
            _logger.Error(
                "{Command} error: {Error}",
                nameof(UpdateParserProfileLinksCommand),
                error
            );
            return new Error(error);
        }

        Result<ParserProfile> profile = await _readRepository.GetById(command.ProfileId, ct);
        if (profile.IsFailure)
        {
            _logger.Error(
                "{Command} error: {Error}",
                nameof(UpdateParserProfileLinksCommand),
                profile.Error.Description
            );
            return profile.Error;
        }

        List<ParserProfileLink> links = [];
        foreach (var link in command.Links)
        {
            Result<ParserProfileLinkBody> body = ParserProfileLinkBody.Create(link.Mark, link.Link);
            if (body.IsFailure)
            {
                _logger.Error(
                    "{Command} error: {Error}",
                    nameof(UpdateParserProfileLinksCommand),
                    body.Error.Description
                );
                return body.Error;
            }

            ParserProfileLinkId? profileLinkId = string.IsNullOrWhiteSpace(link.Id)
                ? null
                : new(GuidUtils.FromString(link.Id));
            links.Add(new ParserProfileLink(body, profileLinkId));
        }

        profile.Value.UpdateProfileLinks(links);
        Result<Guid> update = await _commandRepository.Update(profile, ct);
        return update;
    }
}
