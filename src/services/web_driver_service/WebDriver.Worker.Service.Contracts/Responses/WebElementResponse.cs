namespace WebDriver.Worker.Service.Contracts.Responses;

public record WebElementResponse(Guid ElementId, string OuterHTML, string InnerText);
