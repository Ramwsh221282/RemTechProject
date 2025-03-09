using AvitoParser.PDK.Dtos;
using PuppeteerSharp;
using PuppeteerSharp.Input;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace LoginInAvitoPlugin;

[Plugin(PluginName = nameof(LoginInAvitoPlugin))]
public sealed class LoginInAvitoPlugin : IPlugin<IPage>
{
    private const string script =
        @"() => {    
    Object.defineProperty(navigator, 'webdriver', { get: () => false });
    
    window.chrome = { runtime: {} };
    
    Object.defineProperty(navigator, 'languages', {get: () => ['ru-RU', 'ru']});
    Object.defineProperty(navigator, 'plugins', {
        get: () => [1, 2, 3, 4, 5],
    });
    
    const originalQuery = window.navigator.permissions.query;
    window.navigator.permissions.query = (parameters) => (
        parameters.name === 'notifications' ?
        Promise.resolve({ state: Notification.permission }) :
        originalQuery(parameters)
    );
    
    const getParameter = WebGLRenderingContext.prototype.getParameter;
    WebGLRenderingContext.prototype.getParameter = function(parameter) {
        if (parameter === 37445) {
            return 'Intel Inc.';
        }
        if (parameter === 37446) {
            return 'Intel Iris OpenGL Engine';
        }
        return getParameter(parameter);
    };
}";

    private const string authUrl = "https://www.avito.ru/#login?authsrc=h";

    public async Task<Result<IPage>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(LoginInAvitoPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<LoginDto> loginDataUnwrap = resolver.Resolve<LoginDto>();
        if (loggerUnwrap.IsFailure)
            logger.PluginDependencyMissingError(nameof(LoginInAvitoPlugin), nameof(LoginDto));
        LoginDto loginData = loginDataUnwrap.Value;

        if (string.IsNullOrWhiteSpace(loginData.Email))
        {
            logger.Error("{Context} failed to login. Email is missing", nameof(LoginInAvitoPlugin));
            return new Error("Email is missing");
        }

        if (string.IsNullOrWhiteSpace(loginData.Password))
        {
            logger.Error(
                "{Context} failed to login. Password is missing",
                nameof(LoginInAvitoPlugin)
            );
            return new Error("Password is required for auth");
        }

        Result<IBrowser> browserUnwrap = resolver.Resolve<IBrowser>();
        if (browserUnwrap.IsFailure)
            logger.PluginDependencyMissingError(nameof(LoginInAvitoPlugin), nameof(IBrowser));
        IBrowser browser = browserUnwrap.Value;

        IPage page = await browser.NewPageAsync();
        await page.EvaluateFunctionOnNewDocumentAsync(script);
        await page.GoToAsync(
            authUrl,
            new NavigationOptions()
            {
                Referer = "https://www.avito.ru",
                WaitUntil = [WaitUntilNavigation.DOMContentLoaded],
            }
        );

        int attempts = 0;
        IElementHandle? emailInputElement = null;
        while (attempts < 360 && emailInputElement == null)
        {
            emailInputElement = await page.QuerySelectorAsync(
                "input[data-marker='login-form/login/input']"
            );
            if (emailInputElement == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                attempts++;
            }
            else
                break;
        }

        if (emailInputElement == null)
        {
            logger.Error("{Context} failed to get email input element", nameof(LoginInAvitoPlugin));
            return new Error("Failed to get email input element");
        }

        await emailInputElement.TypeAsync(loginData.Email, new TypeOptions() { Delay = 500 });

        IElementHandle? passwordInputElement = await page.QuerySelectorAsync(
            "input[data-marker='login-form/password/input']"
        );
        if (passwordInputElement == null)
        {
            logger.Error(
                "{Context} failed to get password input element",
                nameof(LoginInAvitoPlugin)
            );
            return new Error("Failed to get password input element");
        }

        await passwordInputElement.TypeAsync(loginData.Password, new TypeOptions() { Delay = 500 });

        IElementHandle? authButtonElement = null;
        attempts = 0;
        while (attempts < 360 && authButtonElement == null)
        {
            authButtonElement = await page.QuerySelectorAsync(
                "button[data-marker='login-form/submit']"
            );
            if (authButtonElement == null)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                attempts++;
            }
            else
                break;
        }

        if (authButtonElement == null)
        {
            logger.Error("{Context} failed to get auth button element", nameof(LoginInAvitoPlugin));
            return new Error("Failed to get auth button element");
        }

        await authButtonElement.ClickAsync();
        IResponse response = await page.WaitForNavigationAsync(
            new NavigationOptions()
            {
                WaitUntil = [WaitUntilNavigation.Networkidle0],
                Timeout = 300000,
            }
        );
        if (response.Ok)
            return Result<IPage>.Success(page);

        logger.Error(
            "{Context} error occured during login. Maybe auth data is invalid",
            nameof(LoginInAvitoPlugin)
        );
        return new Error("Error occured during login. Maybe auth data is invalid");
    }
}
