using GuardValidationLibrary.GuardedFactory;
using RemTech.MainApi.ParsersManagement.Dtos;
using RemTech.MainApi.ParsersManagement.Models;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.UpdateParser.Decorators;

public sealed class UpdateParserValidator(
    IRequestHandler<UpdateParserCommand, Result<ParserResponse>> handler,
    UpdateParserContext context
) : IRequestHandler<UpdateParserCommand, Result<ParserResponse>>
{
    private readonly IRequestHandler<UpdateParserCommand, Result<ParserResponse>> _handler =
        handler;
    private readonly UpdateParserContext _context = context;

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
            return new Error(create.Error);

        _context.UpdatedModel = Option<Parser>.Some(create.Object);
        return await _handler.Handle(new UpdateParserCommand(dto), ct);
    }
}
