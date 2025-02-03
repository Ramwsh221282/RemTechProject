using OpenQA.Selenium.Support.Extensions;
using RemTech.Parser.Contracts.Contracts.Commands;
using RemTech.Parser.Implementation.Core;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Commands.ScrollToTop;

public sealed class ScrollToTopCommandHandler
    : BaseWebDriverCommand,
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
            return WebDriverPluginErrors.Disposed.LogAndReturn(_logger);

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

        Result result = Result.Success().LogAndReturn(_logger, "Executed Scroll To Top Command");
        return await Task.FromResult(result);
    }
}
