using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.DTO;
using WebDriver.Application.Queries.GetElementsInsideOfElement;
using WebDriver.Core.Models;
using WebDriver.Worker.Service.Responses;

namespace WebDriver.Worker.Service.Contracts.GetMultipleChildren;

internal sealed record GetMultipleChildrenContract(Guid ParentId, string Path, string Type)
    : IContract;

internal sealed class GetMultipleChildrenContractHandler(WebDriverApi api)
    : IContractHandler<GetMultipleChildrenContract>
{
    public async Task<ContractActionResult> Handle(GetMultipleChildrenContract contract)
    {
        ExistingElementDTO existing = new(contract.ParentId);
        ElementPathDataDTO path = new(contract.Path, contract.Type);

        GetElementsInsideOfElementQuery query = new(existing, path);
        Result<WebElementObject[]> result = await api.ExecuteQuery<
            GetElementsInsideOfElementQuery,
            WebElementObject[]
        >(query);

        if (result.IsFailure)
            return ContractActionResult.Fail(result.Error.Description);

        WebElementResponse[] items = result
            .Value.Select(i => new WebElementResponse(
                i.ElementId,
                i.ElementOuterHTMLBytes,
                i.ElementInnerTextBytes
            ))
            .ToArray();

        return ContractActionResult.Success(items);
    }
}
