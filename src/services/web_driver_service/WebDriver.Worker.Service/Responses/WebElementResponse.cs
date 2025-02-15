namespace WebDriver.Worker.Service.Responses;

internal record WebElementResponse(
    string ElementPath,
    string ElementPathType,
    Guid ElementId,
    string ElementOuterHTML,
    string ElementInnerText
);
