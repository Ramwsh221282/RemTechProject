using GuardValidationLibrary.GuardedFactory;
using RemTech.MainApi.ParsersManagement.Models;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.CreateParser.Decorators;

public sealed class CreateParserValidating(
    IRequestHandler<CreateParserCommand, Result<Parser>> handler,
    CreateParserContext context
) : IRequestHandler<CreateParserCommand, Result<Parser>>
{
    private readonly IRequestHandler<CreateParserCommand, Result<Parser>> _handler = handler;
    private readonly CreateParserContext _context = context;

    public async Task<Result<Parser>> Handle(
        CreateParserCommand command,
        CancellationToken ct = default
    )
    {
        var create = GuardedCreator.Create<Parser>([new ParserName(command.ParserName)]);
        return await ResultExtensions
            .When<Parser>(!create.IsSuccess)
            .ApplyError(create.Error)
            .Process(async () =>
            {
                _context.Parser = Option<Parser>.Some(create.Object);
                return await _handler.Handle(command, ct);
            });
    }
}
