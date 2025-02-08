using RemTechCommon.Utils.ResultPattern;

namespace RemTech.WebDriver.Plugin.Core;

internal static class WebDriverPluginErrors
{
    public static readonly Error AlreadyDisposed = new Error(
        "Web Driver instance is already disposed."
    );
    public static readonly Error Disposed = new Error("Web Driver instance is disposed.");
    public static readonly Error CannotGetElement = new Error("Cannot get requested element.");
    public static readonly Error CannotDoAction = new Error("Cannot do action.");
    public static readonly Error UnsupportedRequest = new Error("Unsupported request.");
}
