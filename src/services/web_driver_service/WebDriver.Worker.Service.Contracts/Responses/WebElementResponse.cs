namespace WebDriver.Worker.Service.Contracts.Responses;

public record WebElementResponse(string ElementPath, string ElementPathType, Guid ElementId);
