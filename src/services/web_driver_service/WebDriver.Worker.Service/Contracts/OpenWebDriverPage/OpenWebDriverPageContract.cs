using Rabbit.RPC.Server.Abstractions.Communication;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Application;
using WebDriver.Application.Commands.OpenPage;
using WebDriver.Application.DTO;

namespace WebDriver.Worker.Service.Contracts.OpenWebDriverPage;

internal sealed record OpenWebDriverPageContract(string Url) : IContract;

internal sealed class OpenWebDriverPageContractHandler(WebDriverApi api)
    : IContractHandler<OpenWebDriverPageContract>
{
    public async Task<ContractActionResult> Handle(OpenWebDriverPageContract contract)
    {
        WebPageDataDTO data = new(contract.Url);
        OpenPageCommand command = new(data);

        Result opening = await api.ExecuteCommand(command);

        return opening.IsFailure
            ? ContractActionResult.Fail(opening.Error.Description)
            : ContractActionResult.Success(true);
    }
}
