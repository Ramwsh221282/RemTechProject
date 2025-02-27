using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Contracts.Common.Dto.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks;
using RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks.ValueObjects;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.ParserProfileManagement.UpdateParserProfileLinks;

public sealed record UpdateParserProfileLinksCommand(ParserProfileDto Dto) : IAvitoCommand;

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

        var profile = await _readRepository.GetById(command.Dto.Id, ct);
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
        foreach (var link in command.Dto.Links)
        {
            var body = ParserProfileLinkBody.Create(link.Mark, link.Link);
            if (body.IsFailure)
            {
                _logger.Error(
                    "{Command} error: {Error}",
                    nameof(UpdateParserProfileLinksCommand),
                    body.Error.Description
                );
                return body.Error;
            }

            var profileLinkId = string.IsNullOrWhiteSpace(link.Id)
                ? null
                : new ParserProfileLinkId(GuidUtils.FromString(link.Id));
            links.Add(new ParserProfileLink(body, profileLinkId));
        }

        var updated = profile.Value.Update(profile.Value.Name, links, command.Dto.State);
        var update = await _commandRepository.Update(updated, ct);
        return update;
    }
}
