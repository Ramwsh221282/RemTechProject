using OpenQA.Selenium.Support.Extensions;
using RemTech.Parser.Contracts.Contracts.Commands;
using RemTech.Parser.Implementation.Core;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Commands.ScrollToDown;

public sealed class ScrollToDownHandlerHandler
    : BaseWebDriverHandler,
        IWebDriverCommandHandler<ScrollToDownCommand>
{
    private const string GetCurrentHeightScript =
        "return Math.max(document.documentElement.scrollHeight, document.body.scrollHeight);";

    private const string ScrollToBottomScript =
        "window.scrollTo(0, document.documentElement.scrollHeight);";

    public ScrollToDownHandlerHandler(WebDriverInstance instance, ILogger logger)
        : base(instance, logger) { }

    public async Task<Result> Handle(ScrollToDownCommand command)
    {
        if (_instance.Instance == null || _instance.IsDisposed)
        {
            Error error = WebDriverPluginErrors.Disposed;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        bool isScrolled = false;
        long initialHeight = _instance.Instance.ExecuteJavaScript<long>(GetCurrentHeightScript);

        try
        {
            while (!isScrolled)
            {
                _instance.Instance.ExecuteJavaScript(ScrollToBottomScript);
                long currentHeight = _instance.Instance.ExecuteJavaScript<long>(
                    GetCurrentHeightScript
                );
                if (!IsEndOfPage(ref initialHeight, ref currentHeight))
                    continue;
                isScrolled = true;
            }
        }
        catch (Exception ex)
        {
            Error error = new Error($"Error occured: {ex.Message}");
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information("Page scrolled to Down");
        return await Task.FromResult(Result.Success());
    }

    private static bool IsEndOfPage(ref long initialHeight, ref long currentHeight) =>
        currentHeight == initialHeight;
}
