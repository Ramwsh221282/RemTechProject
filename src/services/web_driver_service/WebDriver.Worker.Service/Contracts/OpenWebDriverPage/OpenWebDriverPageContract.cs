using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Commands.OpenPage;
using WebDriver.Application.DTO;

namespace WebDriver.Worker.Service.Contracts.OpenWebDriverPage;

internal sealed record OpenWebDriverPageContract(string Url) : IContract;

internal sealed record OpenWebDriverPageResponse(string OpenedUrl);

internal sealed class OpenWebDriverPageContractHandler : IContractHandler<OpenWebDriverPageContract>
{
    private readonly WebDriverApi _api;

    public OpenWebDriverPageContractHandler(WebDriverApi api) => _api = api;

    public async Task<ContractActionResult> Handle(OpenWebDriverPageContract contract)
    {
        WebPageDataDTO data = new(contract.Url);
        OpenPageCommand command = new(data);

        Result opening = await _api.ExecuteCommand(command);
        if (opening.IsFailure)
            return new ContractActionResult(opening.Error.Description);

        OpenWebDriverPageResponse response = new(contract.Url);
        return new ContractActionResult(response);
    }
}
