using Rabbit.RPC.Server.Abstractions.Communication;
using RemTech.MongoDb.Service.Common.Models.ParsersManagement;
using RemTech.MongoDb.Service.Features.ParserManagement.GetParser;

namespace RemTech.MongoDb.Service.Features.ParserManagement.GetAllParsers;

public sealed record GetAllParsersMessage : IContract;

public sealed class GetAllParsersMessageHandler : IContractHandler<GetAllParsersMessage>
{
    private readonly ParserRepository _repository;

    public GetAllParsersMessageHandler(ParserRepository repository)
    {
        _repository = repository;
    }

    public async Task<ContractActionResult> Handle(GetAllParsersMessage contract)
    {
        List<Parser> parsers = await _repository.GetAll();
        ParserDaoResponse[] response = parsers.Select(p => p.ToParserDaoResponse()).ToArray();
        return ContractActionResult.Success(response);
    }
}
