﻿using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.ParsersManagement.Dtos;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.UpdateParser;

public sealed record UpdateParserMessage(ParserDto Parser) : IContract;

public sealed record UpdateParserCommand(ParserDto NewModel) : IRequest<Result<ParserResponse>>;

public sealed class UpdateParserCommandHandler(
    DataServiceMessagerFactory factory,
    UpdateParserContext context
) : IRequestHandler<UpdateParserCommand, Result<ParserResponse>>
{
    private readonly DataServiceMessagerFactory _factory = factory;
    private readonly UpdateParserContext _context = context;

    public async Task<Result<ParserResponse>> Handle(
        UpdateParserCommand command,
        CancellationToken ct = default
    )
    {
        if (_context.Error.HasValue)
            return _context.Error.Value;

        if (!_context.UpdatedModel.HasValue)
            return new Error("Cannot update parser configuration. Internal server error.");

        ParserDto dto = _context.UpdatedModel.Value.ToDto();
        UpdateParserMessage message = new UpdateParserMessage(dto);
        ContractActionResult updating = await _factory.Create().Send(message, ct);
        return updating.IsSuccess switch
        {
            true => _context.UpdatedModel.Value.ToResponse(),
            false => new Error(updating.Error),
        };
    }
}
