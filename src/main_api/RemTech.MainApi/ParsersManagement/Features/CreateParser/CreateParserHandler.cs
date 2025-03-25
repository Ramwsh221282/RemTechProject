using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.ParsersManagement.Dtos;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Models;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.CreateParser;

public sealed record SaveParserMessage(ParserDto Parser) : IContract;

public sealed class CreateParserHandler(DataServiceMessager messager, CreateParserContext context)
    : IRequestHandler<CreateParserCommand, Result<Parser>>
{
    private readonly DataServiceMessager _messager = messager;
    private readonly CreateParserContext _context = context;

    public async Task<Result<Parser>> Handle(
        CreateParserCommand command,
        CancellationToken ct = default
    )
    {
        if (!_context.Parser.HasValue)
            return _context.Error.Value;

        Parser parser = _context.Parser.Value;
        ParserDto dto = parser.ToDto();
        SaveParserMessage message = new SaveParserMessage(dto);
        ContractActionResult saving = await _messager.Send(message, ct);
        return saving.IsSuccess ? parser : new Error(saving.Error);
    }
}
