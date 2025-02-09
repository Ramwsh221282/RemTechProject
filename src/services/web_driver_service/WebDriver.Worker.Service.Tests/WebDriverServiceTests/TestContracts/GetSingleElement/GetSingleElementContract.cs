using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.GetSingleElement;

public record GetSingleElementContract(string ElementPath, string ElementPathType) : IContract;

public record WebElementResponse(string ElementPath, string ElementPathType, Guid ElementId);
