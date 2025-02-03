using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Implementation.Core;

public static class WebDriverPluginErrors
{
    public static readonly Error AlreadyDisposed = new Error(
        "Web Driver instance is already disposed."
    );
    public static readonly Error Disposed = new Error("Web Driver instance is disposed.");
    public static readonly Error CannotGetElement = new Error("Cannot get requested element.");
    public static readonly Error CannotDoAction = new Error("Cannot do action.");
}
