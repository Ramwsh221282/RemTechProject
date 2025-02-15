using System.Text;

namespace WebDriver.Worker.Service.Contracts.Responses;

public record WebElementResponse(
    Guid ElementId,
    ReadOnlyMemory<byte> ElementOuterHTMLBytes,
    ReadOnlyMemory<byte> ElementInnterTextBytes
)
{
    public string ElementOuterHTML =>
        ElementOuterHTMLBytes.IsEmpty
            ? string.Empty
            : Encoding.UTF8.GetString(ElementOuterHTMLBytes.Span);

    public string ElementInnerText =>
        ElementInnterTextBytes.IsEmpty
            ? string.Empty
            : Encoding.UTF8.GetString(ElementInnterTextBytes.Span);
}
