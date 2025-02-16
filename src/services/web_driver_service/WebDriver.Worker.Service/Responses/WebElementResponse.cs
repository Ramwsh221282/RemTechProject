using WebDriver.Core.Models;

namespace WebDriver.Worker.Service.Responses;

internal record WebElementResponse(Guid ElementId, string OuterHTML, string InnerText)
{
    public static implicit operator WebElementResponse(WebElementResponseObject internalObject)
    {
        return new WebElementResponse(
            internalObject.Id,
            internalObject.OuterHTML,
            internalObject.InnerText
        );
    }
}
