using Rabbit.RPC.Server.Abstractions.Communication;
using RemTech.MongoDb.Service.Common.Models.ParsersManagement;
using ILogger = Serilog.ILogger;

namespace RemTech.MongoDb.Service.Features.ParserManagement.ParserSave;

public sealed record SaveParserMessage(ParserDto Parser) : IContract;

public sealed class SaveParserContractHandler : IContractHandler<SaveParserMessage>
{
    private readonly ILogger _logger;
    private readonly ParserRepository _repository;

    public SaveParserContractHandler(ILogger logger, ParserRepository repository) =>
        (_logger, _repository) = (logger, repository);

    public async Task<ContractActionResult> Handle(SaveParserMessage contract)
    {
        Parser parser = contract.Parser.MapToParserModel();
        await _repository.Add(parser);
        _logger.Information("Parser {Parser} saved.", contract.Parser);
        return ContractActionResult.Success(true);
    }
}
