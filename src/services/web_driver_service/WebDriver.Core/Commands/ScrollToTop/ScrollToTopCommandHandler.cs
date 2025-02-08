using OpenQA.Selenium.Support.Extensions;
using RemTech.WebDriver.Plugin.Core;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.WebDriver.Plugin.Commands.ScrollToTop;

internal sealed class ScrollToTopCommandHandler
    : BaseWebDriverHandler,
        IWebDriverCommandHandler<ScrollToTopCommand>
{
    private const string Script = "window.scrollTo(0, 0);";

    private const string GetScrollPositionScript =
        "return window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;";

    public ScrollToTopCommandHandler(WebDriverInstance instance, ILogger logger)
        : base(instance, logger) { }

    public async Task<Result> Handle(ScrollToTopCommand command)
    {
        if (_instance.Instance == null || _instance.IsDisposed)
        {
            Error error = WebDriverPluginErrors.Disposed;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        bool isScrolled = false;
        while (!isScrolled)
        {
            long currentHeight = _instance.Instance.ExecuteJavaScript<long>(
                GetScrollPositionScript
            );

            if (currentHeight == 0)
                isScrolled = true;

            _instance.Instance.ExecuteJavaScript(Script);
        }

        _logger.Information("Executed Scroll To Top Command");
        return await Task.FromResult(Result.Success());
    }
}
