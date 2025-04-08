using RemTech.Application.ParserContext.Contracts;
using RemTech.Domain.ParserContext;

namespace RemTech.Application.ParserContext.Features.RemoveParserProfile;

public sealed class RemoveParserProfileCommandHandler(IParserWriteRepository repository)
    : ICommandHandler<RemoveParserProfileCommand, UnitResult<Guid>>
{
    private readonly IParserWriteRepository _repository = repository;

    public async Task<UnitResult<Guid>> Handle(
        RemoveParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        (string parserName, string profileName) = command;

        Option<Parser> parser = await _repository.GetByName(parserName, ct);
        if (parser.HasValue == false)
            return UnitResult<Guid>.FromFailure(
                new Error($"Не найден парсер: {parserName}"),
                UnitResultCodes.NotFound
            );

        Result<Guid> removing = parser.Value.RemoveProfile(profileName);

        if (removing.IsFailure)
            return UnitResult<Guid>.FromFailure(removing.Error, UnitResultCodes.BadRequest);

        await _repository.Save(parser.Value, ct);
        return parser.Value.Id.Id;
    }
}
