using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Core;

public sealed class WebDriverFactory
{
    private readonly ILogger _logger;

    private readonly WebDriverInstanceOptions _options;

    private readonly string _chromeExecutablePath =
        WebDriverPluginConstants.GetExpectedChromeDriverExecutablePath();

    public WebDriverFactory(ILogger logger, WebDriverInstanceOptions options)
    {
        _logger = logger;
        _options = options;
        logger.Information("Web Driver Factory created");
    }

    public Result<IWebDriver> Create()
    {
        bool isExists = EnsureChromeDriverExists();
        if (!isExists)
            return new Error(
                "Cannot instantiate web driver instance. No web driver executable exists"
            );

        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        service.HideCommandPromptWindow = true;
        service.EnableVerboseLogging = false;
        service.EnableAppendLog = false;

        IWebDriver driver = new ChromeDriver(service, _options.Options);
        return Result<IWebDriver>.Success(driver);
    }

    private bool EnsureChromeDriverExists()
    {
        try
        {
            if (File.Exists(_chromeExecutablePath))
                return true;
            _logger.Error("No web driver chrome directories");
            return false;
        }
        catch
        {
            _logger.Error("Web driver executable doesn't exist");
            return false;
        }
    }
}
