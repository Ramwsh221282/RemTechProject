using MongoDB.Driver;
using Rabbit.RPC.Server.Abstractions.Communication;
using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Models.ParsersManagement;
using RemTech.MongoDb.Service.Features.ParserManagement.ParserQuerying;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MongoDb.Service.Features.ParserManagement.ParserDelete;

public record class DeleteParserMessage(ParserQueryPayload Payload) : IContract;

public sealed class DeleteParserContractHandler : IContractHandler<DeleteParserMessage>
{
    private readonly IQueryBuilder<ParserQueryPayload, Parser> _builder;
    private readonly ParserRepository _repository;

    public DeleteParserContractHandler(
        IQueryBuilder<ParserQueryPayload, Parser> builder,
        ParserRepository repository
    )
    {
        _builder = builder;
        _repository = repository;
    }

    public async Task<ContractActionResult> Handle(DeleteParserMessage contract)
    {
        ParserQueryPayload payload = contract.Payload;
        _builder.SetPayload(payload);
        FilterDefinition<Parser> filter = _builder.Build();

        Result contains = await _repository.Contains(filter);
        if (contains.IsFailure)
            return ContractActionResult.Fail(contains.Error.Description);

        Result deleted = await _repository.Delete(filter);
        return deleted.IsFailure
            ? ContractActionResult.Fail(deleted.Error.Description)
            : ContractActionResult.Success(true);
    }
}
