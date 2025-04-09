using PuppeteerSharp;
using RemTech.Shared.SDK.OptionPattern;
using RemTech.Shared.SDK.ResultPattern;
using SharedParsersLibrary.Puppeteer.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Extensions;
using SharedParsersLibrary.Puppeteer.PageBehavior;

namespace RemTech.Parser.Avito.Scraping.Authorization;

public sealed class AvitoAuthorization(IBrowser browser)
{
    private readonly IBrowser _browser = browser;
    private const string AUTH_URL = "https://www.avito.ru/#login?authsrc=h";
    private const string authFormSelector = "form[data-marker='login-form']";
    private const string loginInputSelector = "input[data-marker='login-form/login/input']";
    private const string passwordInputSelector = "input[data-marker='login-form/password/input']";
    private const string submitButtonSelector = "button[data-marker='login-form/submit']";
    private const string userProfileSelector = "a[data-marker='header/username-button']";
    private const string fireWallTitleSelector = "h2[class='firewall-title']";

    private const string test_mail = "jimkrauz@gmail.com";
    private const string test_password = "9595330zxzO!";

    public async Task<Result> Authorize(int maxRetryCount = 3)
    {
        for (int i = 0; i < maxRetryCount; i++)
        {
            Result result = await Invoke();
            if (result.IsSuccess)
                return result;
        }

        return new Error("Unable to authorize");
    }

    private async Task<Result> Invoke()
    {
        IPage page = await _browser.CreateByDomLoadStrategy(AUTH_URL);
        if (await IsBanned(page))
            await RefreshPage(page);

        try
        {
            Option<IElementHandle> authForm = await page.GetElementWithoutClassFormatter(
                authFormSelector
            );
            if (authForm.HasValue == false)
                return new Error("Auth page form was not found");

            Option<IElementHandle> login = await authForm.Value.GetChildWithoutClassFormatter(
                loginInputSelector
            );
            if (login.HasValue == false)
                return new Error("Login input was not found");

            await login.Value.TypeAsync(test_mail);

            Option<IElementHandle> password = await authForm.Value.GetChildWithoutClassFormatter(
                passwordInputSelector
            );
            if (password.HasValue == false)
                return new Error("Password input was not found");

            await password.Value.TypeAsync(test_password);

            Option<IElementHandle> submitButton =
                await authForm.Value.GetChildWithoutClassFormatter(submitButtonSelector);
            if (submitButton.HasValue == false)
                return new Error("Submit button was not found");

            Task<IResponse>? navigationTask = page.WaitForNavigationAsync(
                new NavigationOptions { WaitUntil = [WaitUntilNavigation.DOMContentLoaded] }
            );
            await submitButton.Value.ClickAsync();
            await navigationTask;

            bool isAuthorized = await IsAuthorized(page);

            page.Dispose();

            return isAuthorized ? Result.Success() : new Error("Unable to authorize.");
        }
        catch
        {
            return new Error("Unable to authorize.");
        }
        finally
        {
            page.Dispose();
        }
    }

    private static async Task<bool> IsAuthorized(IPage page)
    {
        Option<IElementHandle> element = await page.GetElementWithoutClassFormatter(
            userProfileSelector
        );
        return element.HasValue;
    }

    private static async Task<bool> IsBanned(IPage page)
    {
        Option<IElementHandle> fireWall = await page.GetElementWithoutClassFormatter(
            fireWallTitleSelector
        );
        return fireWall.HasValue;
    }

    private static async Task RefreshPage(IPage page)
    {
        try
        {
            await page.ReloadAsync();
            await page.WaitForNavigationAsync(
                new NavigationOptions() { WaitUntil = [WaitUntilNavigation.DOMContentLoaded] }
            );
        }
        catch { }
    }
}
