using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Queries.GetPageHtml;

namespace WebDriver.Worker.Service.Contracts.GetPageHtml;

internal sealed record GetPageHtmlContract : IContract;

internal sealed class GetPageHtmlContractHandler(WebDriverApi api)
    : IContractHandler<GetPageHtmlContract>
{
    public async Task<ContractActionResult> Handle(GetPageHtmlContract contract)
    {
        GetPageHtmlQuery query = new();
        Result<string> html = await api.ExecuteQuery<GetPageHtmlQuery, string>(query);

        return html.IsFailure
            ? ContractActionResult.Fail(html.Error.Description)
            : ContractActionResult.Success(html.Value);
    }
}
