using GuardValidationLibrary.GuardedFactory;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.ParsersManagement.Models;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.CreateParser.Decorators;

public sealed class CreateParserValidating : ICommandHandler<CreateParserCommand, Parser>
{
    private readonly ICommandHandler<CreateParserCommand, Parser> _handler;
    private readonly CreateParserContext _context;

    public CreateParserValidating(CreateParserContext context, CreateParserHandler handler)
    {
        _context = context;
        _handler = handler;
    }

    public async Task<Result<Parser>> Handle(
        CreateParserCommand command,
        CancellationToken ct = default
    )
    {
        var create = GuardedCreator.Create<Parser>([new ParserName(command.ParserName)]);
        if (!create.IsSuccess)
            _context.Error = Option<Error>.Some(new Error(create.Error));

        _context.Parser = Option<Parser>.Some(create.Object);
        return await _handler.Handle(command, ct);
    }
}
