namespace WebDriver.Worker.Service.Responses;

internal record WebElementResponse(
    Guid ElementId,
    ReadOnlyMemory<byte> ElementOuterHTMLBytes,
    ReadOnlyMemory<byte> ElementInnterTextBytes
);
