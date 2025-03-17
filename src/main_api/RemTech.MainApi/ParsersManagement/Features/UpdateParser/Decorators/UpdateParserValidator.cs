using GuardValidationLibrary.GuardedFactory;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.ParsersManagement.Dtos;
using RemTech.MainApi.ParsersManagement.Models;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.UpdateParser.Decorators;

public sealed class UpdateParserValidator : ICommandHandler<UpdateParserCommand, ParserResponse>
{
    private readonly ICommandHandler<UpdateParserCommand, ParserResponse> _handler;
    private readonly UpdateParserContext _context;

    public UpdateParserValidator(
        ICommandHandler<UpdateParserCommand, ParserResponse> handler,
        UpdateParserContext context
    )
    {
        _handler = handler;
        _context = context;
    }

    public async Task<Result<ParserResponse>> Handle(
        UpdateParserCommand command,
        CancellationToken ct = default
    )
    {
        ParserDto dto = command.NewModel;
        GuardedCreation<Parser> create = GuardedCreator.Create<Parser>(
            new ParserName(dto.ParserName),
            new ParserSchedule(dto.RepeatEveryHours, dto.LastRun, dto.NextRun),
            new ParserState(dto.ParserState),
            dto.Links.Select(l => new ParserLink(l)).ToArray()
        );

        if (!create.IsSuccess)
            _context.Error = Option<Error>.Some(new Error(create.Error));

        _context.UpdatedModel = Option<Parser>.Some(create.Object);
        return await _handler.Handle(new UpdateParserCommand(dto), ct);
    }
}
