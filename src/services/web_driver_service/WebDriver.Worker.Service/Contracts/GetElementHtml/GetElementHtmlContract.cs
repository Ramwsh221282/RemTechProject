using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.DTO;
using WebDriver.Application.Queries.GetElementHtml;

namespace WebDriver.Worker.Service.Contracts.GetElementHtml;

internal sealed record GetElementHtmlContract(Guid ExistingId) : IContract;

internal sealed class GetElementHtmlContractHandler(WebDriverApi api)
    : IContractHandler<GetElementHtmlContract>
{
    public async Task<ContractActionResult> Handle(GetElementHtmlContract contract)
    {
        ExistingElementDTO data = new(contract.ExistingId);
        GetElementHtmlQuery query = new(data);
        Result<string> html = await api.ExecuteQuery<GetElementHtmlQuery, string>(query);

        return html.IsFailure
            ? ContractActionResult.Fail(html.Error.Description)
            : ContractActionResult.Success(html.Value);
    }
}
