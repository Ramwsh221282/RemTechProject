using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Queries.GetPageHtml;

namespace WebDriver.Worker.Service.Contracts.GetPageHtml;

internal sealed record GetPageHtmlContract : IContract;

internal sealed record GetHtmlResponse(string Html);

internal sealed class GetPageHtmlContractHandler(WebDriverApi api)
    : IContractHandler<GetPageHtmlContract>
{
    public async Task<ContractActionResult> Handle(GetPageHtmlContract contract)
    {
        GetPageHtmlQuery query = new();
        Result<string> html = await api.ExecuteQuery<GetPageHtmlQuery, string>(query);

        if (html.IsFailure)
            return new ContractActionResult(html.Error.Description);

        GetHtmlResponse response = new(html.Value);
        return new ContractActionResult(response);
    }
}
