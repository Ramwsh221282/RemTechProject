using MongoDB.Driver;
using Rabbit.RPC.Server.Abstractions.Communication;
using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Models.ParsersManagement;
using RemTech.MongoDb.Service.Features.ParserManagement.ParserQuerying;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MongoDb.Service.Features.ParserManagement.UpdateParser;

public sealed record UpdateParserMessage(ParserDto Parser) : IContract;

public sealed class UpdateParserHandler : IContractHandler<UpdateParserMessage>
{
    private readonly IQueryBuilder<ParserQueryPayload, Parser> _queryBuilder;
    private readonly ParserRepository _repository;

    public UpdateParserHandler(
        ParserRepository repository,
        IQueryBuilder<ParserQueryPayload, Parser> queryBuilder
    )
    {
        _repository = repository;
        _queryBuilder = queryBuilder;
    }

    public async Task<ContractActionResult> Handle(UpdateParserMessage contract)
    {
        ParserDto dto = contract.Parser;
        ParserQueryPayload payload = new(ServiceName: dto.ParserName);
        Parser parser = dto.MapToParserModel();

        _queryBuilder.SetPayload(payload);
        FilterDefinition<Parser> filter = _queryBuilder.Build();
        Result result = await _repository.Update(filter, parser);

        return result.IsSuccess
            ? ContractActionResult.Success(true)
            : ContractActionResult.Fail(result.Error.Description);
    }
}
