using MongoDB.Driver;
using Rabbit.RPC.Server.Abstractions.Communication;
using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Models.ParsersManagement;
using RemTech.MongoDb.Service.Features.ParserManagement.ParserQuerying;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MongoDb.Service.Features.ParserManagement.GetParser;

public sealed record ParserDaoResponse(
    string ParserName,
    int RepeatEveryHours,
    DateTime LastRun,
    DateTime NextRun,
    string State,
    string[] Links
);

public sealed record GetParserMessage(ParserQueryPayload Payload) : IContract;

public sealed class GetParserMessageHandler : IContractHandler<GetParserMessage>
{
    private readonly ParserRepository _repository;
    private readonly IQueryBuilder<ParserQueryPayload, Parser> _builder;

    public GetParserMessageHandler(
        ParserRepository repository,
        IQueryBuilder<ParserQueryPayload, Parser> builder
    )
    {
        _repository = repository;
        _builder = builder;
    }

    public async Task<ContractActionResult> Handle(GetParserMessage contract)
    {
        _builder.SetPayload(contract.Payload);
        FilterDefinition<Parser> filter = _builder.Build();
        Result<Parser> parser = await _repository.QuerySingle(filter);
        return parser.IsSuccess
            ? ContractActionResult.Success(parser.Value.ToParserDaoResponse())
            : ContractActionResult.Fail(parser.Error.Description);
    }
}
