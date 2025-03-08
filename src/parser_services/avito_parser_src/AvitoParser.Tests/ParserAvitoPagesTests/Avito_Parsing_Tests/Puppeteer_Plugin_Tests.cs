using AvitoParser.PDK;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerSharp;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace AvitoParser.Tests.ParserAvitoPagesTests.Avito_Parsing_Tests;

public class Puppeteer_Plugin_Tests
{
    private readonly IServiceProvider _provider;

    public Puppeteer_Plugin_Tests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<ILogger>(new LoggerConfiguration().WriteTo.Console().CreateLogger());
        services.AddTransient<PluginExecutionContext>();
        services.AddTransient<PluginFileValidator>();
        services.AddTransient<PluginResolver>();
        _provider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task Test_Scroll_Bottom()
    {
        const string catalogueUrl =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();

        ILogger logger = _provider.GetRequiredService<ILogger>();
        PuppeteerLaunchOptions options = new PuppeteerLaunchOptions(
            Arguments: ["--start-maximized"],
            Headless: false
        );

        PluginCommand instantiateCommand = new PluginCommand(
            "InstantiatePuppeteerPlugin",
            new AvitoPluginPayload(logger, options)
        );

        Result<IBrowser> instantiation = await context.ExecutePlugin<IBrowser>(instantiateCommand);

        Assert.True(instantiation.IsSuccess);

        IBrowser browser = instantiation.Value;
        try
        {
            IPage page = await browser.NewPageAsync();
            await page.GoToAsync(
                catalogueUrl,
                new NavigationOptions() { WaitUntil = [WaitUntilNavigation.DOMContentLoaded] }
            );

            PluginCommand scrollBottomCommand = new PluginCommand(
                "ScrollBottomPlugin",
                new AvitoPluginPayload(logger, page)
            );

            Result scrollingResult = await context.ExecutePlugin(scrollBottomCommand);
            Assert.True(scrollingResult.IsSuccess);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Test} failed. Error: {Message}", nameof(Test_Scroll_Bottom), ex.Message);
        }
        finally
        {
            logger.Information("Stopping browser");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await browser.DisposeAsync();
        }
    }

    [Fact]
    public async Task TestScrollBottomAndTop()
    {
        const string catalogueUrl =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        ILogger logger = _provider.GetRequiredService<ILogger>();
        PuppeteerLaunchOptions options = new PuppeteerLaunchOptions(
            Arguments: ["--start-maximized"],
            Headless: false
        );

        PluginCommand instantiateCommand = new PluginCommand(
            "InstantiatePuppeteerPlugin",
            new AvitoPluginPayload(logger, options)
        );
        Result<IBrowser> instantiation = await context.ExecutePlugin<IBrowser>(instantiateCommand);

        Assert.True(instantiation.IsSuccess);

        IBrowser browser = instantiation.Value;
        try
        {
            IPage page = await browser.NewPageAsync();
            await page.GoToAsync(
                catalogueUrl,
                new NavigationOptions() { WaitUntil = [WaitUntilNavigation.DOMContentLoaded] }
            );

            PluginCommand scrollBottomCommand = new PluginCommand(
                "ScrollBottomPlugin",
                new AvitoPluginPayload(logger, page)
            );
            Result scrollingResult = await context.ExecutePlugin(scrollBottomCommand);
            Assert.True(scrollingResult.IsSuccess);

            PluginCommand scrollTopCommand = new PluginCommand(
                "ScrollTopPlugin",
                new AvitoPluginPayload(logger, page)
            );
            Result scrollingTopResult = await context.ExecutePlugin(scrollTopCommand);
            Assert.True(scrollingTopResult.IsSuccess);
        }
        catch (Exception ex)
        {
            logger.Fatal("{Test} failed. Error: {Message}", nameof(Test_Scroll_Bottom), ex.Message);
        }
        finally
        {
            logger.Information("Stopping browser");
            await Task.Delay(TimeSpan.FromSeconds(5));
            await browser.DisposeAsync();
        }
    }

    [Fact]
    public async Task Get_Element_By_Selector_Test()
    {
        bool noExceptions = true;
        const string catalogueUrl =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        ILogger logger = _provider.GetRequiredService<ILogger>();
        PuppeteerLaunchOptions options = new PuppeteerLaunchOptions(
            Arguments: ["--start-maximized"],
            Headless: false
        );

        PluginCommand instantiateCommand = new PluginCommand(
            "InstantiatePuppeteerPlugin",
            new AvitoPluginPayload(logger, options)
        );
        Result<IBrowser> instantiation = await context.ExecutePlugin<IBrowser>(instantiateCommand);

        Assert.True(instantiation.IsSuccess);

        await using IBrowser browser = instantiation.Value;
        await using IPage page = await browser.NewPageAsync();
        await page.GoToAsync(
            catalogueUrl,
            new NavigationOptions() { WaitUntil = [WaitUntilNavigation.DOMContentLoaded] }
        );

        PluginCommand scrollBottomCommand = new PluginCommand(
            "ScrollBottomPlugin",
            new AvitoPluginPayload(logger, page)
        );
        Result scrollingResult = await context.ExecutePlugin(scrollBottomCommand);
        Assert.True(scrollingResult.IsSuccess);

        PluginCommand scrollTopCommand = new PluginCommand(
            "ScrollTopPlugin",
            new AvitoPluginPayload(logger, page)
        );
        Result scrollingTopResult = await context.ExecutePlugin(scrollTopCommand);
        Assert.True(scrollingTopResult.IsSuccess);

        try
        {
            QuerySelectorPayload querySelector = new QuerySelectorPayload(
                "styles-module-item-z1yUP styles-module-item_size_s-XM_Tf styles-module-item_last-ucP91 styles-module-item_link-Y7Iww"
            );

            PluginCommand getElementCommand = new PluginCommand(
                "GetElementPlugin",
                new AvitoPluginPayload(logger, page, querySelector)
            );

            Result<IElementHandle> element = await context.ExecutePlugin<IElementHandle>(
                getElementCommand
            );

            Assert.True(element.IsSuccess);
            Assert.NotNull(element.Value);
            await element.Value.DisposeAsync();
        }
        catch (Exception ex)
        {
            noExceptions = false;
            logger.Fatal(
                "{Test} failed. Error: {Message}",
                nameof(Get_Element_By_Selector_Test),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Get_Element_Attribute_Test()
    {
        bool noExceptions = true;
        const string catalogueUrl =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        ILogger logger = _provider.GetRequiredService<ILogger>();
        PuppeteerLaunchOptions options = new PuppeteerLaunchOptions(
            Arguments: ["--start-maximized"],
            Headless: false
        );

        PluginCommand instantiateCommand = new PluginCommand(
            "InstantiatePuppeteerPlugin",
            new AvitoPluginPayload(logger, options)
        );
        Result<IBrowser> instantiation = await context.ExecutePlugin<IBrowser>(instantiateCommand);

        Assert.True(instantiation.IsSuccess);

        await using IBrowser browser = instantiation.Value;
        await using IPage page = await browser.NewPageAsync();
        await page.GoToAsync(
            catalogueUrl,
            new NavigationOptions() { WaitUntil = [WaitUntilNavigation.DOMContentLoaded] }
        );

        PluginCommand scrollBottomCommand = new PluginCommand(
            "ScrollBottomPlugin",
            new AvitoPluginPayload(logger, page)
        );
        Result scrollingResult = await context.ExecutePlugin(scrollBottomCommand);
        Assert.True(scrollingResult.IsSuccess);

        PluginCommand scrollTopCommand = new PluginCommand(
            "ScrollTopPlugin",
            new AvitoPluginPayload(logger, page)
        );
        Result scrollingTopResult = await context.ExecutePlugin(scrollTopCommand);
        Assert.True(scrollingTopResult.IsSuccess);

        QuerySelectorPayload querySelector = new QuerySelectorPayload(
            "styles-module-item-z1yUP styles-module-item_size_s-XM_Tf styles-module-item_last-ucP91 styles-module-item_link-Y7Iww"
        );

        PluginCommand getElementCommand = new PluginCommand(
            "GetElementPlugin",
            new AvitoPluginPayload(logger, page, querySelector)
        );

        Result<IElementHandle> elementHandleResult = await context.ExecutePlugin<IElementHandle>(
            getElementCommand
        );
        Assert.True(elementHandleResult.IsSuccess);

        await using IElementHandle elementHandle = elementHandleResult.Value;

        try
        {
            QueryAttributePayload queryAttributePayload = new QueryAttributePayload("data-value");
            PluginCommand getElementAttributeCommand = new PluginCommand(
                "GetElementAttributePlugin",
                new AvitoPluginPayload(elementHandle, logger, queryAttributePayload)
            );
            Result<string> attributeValue = await context.ExecutePlugin<string>(
                getElementAttributeCommand
            );
            Assert.True(attributeValue.IsSuccess);
            Assert.NotEmpty(attributeValue.Value);

            PluginCommand getElementOuterHtmlCommand = new PluginCommand(
                "GetElementOuterHTMLPlugin",
                new AvitoPluginPayload(logger, elementHandle)
            );
            Result<string> outerHTML = await context.ExecutePlugin<string>(
                getElementOuterHtmlCommand
            );
            Assert.True(outerHTML.IsSuccess);
            Assert.NotEmpty(outerHTML.Value);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            logger.Fatal(
                "{Test} failed. Error: {Message}",
                nameof(Get_Element_Attribute_Test),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Get_Element_Children_Test()
    {
        bool noExceptions = true;
        const string catalogueUrl =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        ILogger logger = _provider.GetRequiredService<ILogger>();
        PuppeteerLaunchOptions options = new PuppeteerLaunchOptions(
            Arguments: ["--start-maximized"],
            Headless: false
        );

        PluginCommand instantiateCommand = new PluginCommand(
            "InstantiatePuppeteerPlugin",
            new AvitoPluginPayload(logger, options)
        );
        Result<IBrowser> instantiation = await context.ExecutePlugin<IBrowser>(instantiateCommand);

        Assert.True(instantiation.IsSuccess);

        await using IBrowser browser = instantiation.Value;
        await using IPage page = await browser.NewPageAsync();
        await page.GoToAsync(
            catalogueUrl,
            new NavigationOptions() { WaitUntil = [WaitUntilNavigation.DOMContentLoaded] }
        );

        PluginCommand scrollBottomCommand = new PluginCommand(
            "ScrollBottomPlugin",
            new AvitoPluginPayload(logger, page)
        );
        Result scrollingResult = await context.ExecutePlugin(scrollBottomCommand);
        Assert.True(scrollingResult.IsSuccess);

        PluginCommand scrollTopCommand = new PluginCommand(
            "ScrollTopPlugin",
            new AvitoPluginPayload(logger, page)
        );
        Result scrollingTopResult = await context.ExecutePlugin(scrollTopCommand);
        Assert.True(scrollingTopResult.IsSuccess);

        QuerySelectorPayload querySelector = new QuerySelectorPayload("items-items-pZX46");
        PluginCommand getElementCommand = new PluginCommand(
            "GetElementPlugin",
            new AvitoPluginPayload(logger, page, querySelector)
        );

        Result<IElementHandle> elementHandleResult = await context.ExecutePlugin<IElementHandle>(
            getElementCommand
        );
        Assert.True(elementHandleResult.IsSuccess);

        await using IElementHandle elementHandle = elementHandleResult.Value;

        try
        {
            QuerySelectorPayload querySelectorChildren = new QuerySelectorPayload(
                "iva-item-root-Se7z4"
            );
            PluginCommand getElementChildren = new PluginCommand(
                "GetElementChildrenPlugin",
                new AvitoPluginPayload(logger, elementHandle, querySelectorChildren)
            );
            Result<IEnumerable<IElementHandle>> getElementChildrenResult =
                await context.ExecutePlugin<IEnumerable<IElementHandle>>(getElementChildren);
            Assert.True(getElementChildrenResult.IsSuccess);
            Assert.NotNull(getElementChildrenResult.Value);
            Assert.NotEmpty(getElementChildrenResult.Value);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            logger.Fatal(
                "{Test} failed. Error: {Message}",
                nameof(Get_Element_Attribute_Test),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Get_Element_Text_Content_Test()
    {
        bool noExceptions = true;
        const string catalogueUrl =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        ILogger logger = _provider.GetRequiredService<ILogger>();
        PuppeteerLaunchOptions options = new PuppeteerLaunchOptions(
            Arguments: ["--start-maximized"],
            Headless: false
        );

        PluginCommand instantiateCommand = new PluginCommand(
            "InstantiatePuppeteerPlugin",
            new AvitoPluginPayload(logger, options)
        );
        Result<IBrowser> instantiation = await context.ExecutePlugin<IBrowser>(instantiateCommand);

        Assert.True(instantiation.IsSuccess);

        await using IBrowser browser = instantiation.Value;
        await using IPage page = await browser.NewPageAsync();
        await page.GoToAsync(
            catalogueUrl,
            new NavigationOptions() { WaitUntil = [WaitUntilNavigation.DOMContentLoaded] }
        );

        PluginCommand scrollBottomCommand = new PluginCommand(
            "ScrollBottomPlugin",
            new AvitoPluginPayload(logger, page)
        );
        Result scrollingResult = await context.ExecutePlugin(scrollBottomCommand);
        Assert.True(scrollingResult.IsSuccess);

        PluginCommand scrollTopCommand = new PluginCommand(
            "ScrollTopPlugin",
            new AvitoPluginPayload(logger, page)
        );
        Result scrollingTopResult = await context.ExecutePlugin(scrollTopCommand);
        Assert.True(scrollingTopResult.IsSuccess);

        QuerySelectorPayload querySelector = new QuerySelectorPayload("items-items-pZX46");
        PluginCommand getElementCommand = new PluginCommand(
            "GetElementPlugin",
            new AvitoPluginPayload(logger, page, querySelector)
        );

        Result<IElementHandle> elementHandleResult = await context.ExecutePlugin<IElementHandle>(
            getElementCommand
        );
        Assert.True(elementHandleResult.IsSuccess);

        await using IElementHandle elementHandle = elementHandleResult.Value;

        try
        {
            QuerySelectorPayload querySelectorChildren = new QuerySelectorPayload(
                "iva-item-root-Se7z4"
            );
            PluginCommand getElementChildren = new PluginCommand(
                "GetElementChildrenPlugin",
                new AvitoPluginPayload(logger, elementHandle, querySelectorChildren)
            );
            Result<IEnumerable<IElementHandle>> getElementChildrenResult =
                await context.ExecutePlugin<IEnumerable<IElementHandle>>(getElementChildren);
            Assert.True(getElementChildrenResult.IsSuccess);
            Assert.NotNull(getElementChildrenResult.Value);
            Assert.NotEmpty(getElementChildrenResult.Value);

            IEnumerable<IElementHandle> elementHandles = getElementChildrenResult.Value;
            foreach (var item in elementHandles)
            {
                PluginCommand getTextCommand = new PluginCommand(
                    "GetElementTextContentPlugin",
                    new AvitoPluginPayload(logger, item)
                );
                Result<string> getTextResult = await context.ExecutePlugin<string>(getTextCommand);
                if (getTextResult.IsSuccess)
                    logger.Information(getTextResult.Value);
            }
        }
        catch (Exception ex)
        {
            noExceptions = false;
            logger.Fatal(
                "{Test} failed. Error: {Message}",
                nameof(Get_Element_Attribute_Test),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Click_Element_Test()
    {
        bool noExceptions = true;
        const string catalogueUrl =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        ILogger logger = _provider.GetRequiredService<ILogger>();
        PuppeteerLaunchOptions options = new PuppeteerLaunchOptions(
            Arguments: ["--start-maximized"],
            Headless: false
        );

        PluginCommand instantiateCommand = new PluginCommand(
            "InstantiatePuppeteerPlugin",
            new AvitoPluginPayload(logger, options)
        );
        Result<IBrowser> instantiation = await context.ExecutePlugin<IBrowser>(instantiateCommand);

        Assert.True(instantiation.IsSuccess);

        await using IBrowser browser = instantiation.Value;
        await using IPage page = await browser.NewPageAsync();
        await page.GoToAsync(
            catalogueUrl,
            new NavigationOptions() { WaitUntil = [WaitUntilNavigation.DOMContentLoaded] }
        );

        PluginCommand scrollBottomCommand = new PluginCommand(
            "ScrollBottomPlugin",
            new AvitoPluginPayload(logger, page)
        );
        Result scrollingResult = await context.ExecutePlugin(scrollBottomCommand);
        Assert.True(scrollingResult.IsSuccess);

        PluginCommand scrollTopCommand = new PluginCommand(
            "ScrollTopPlugin",
            new AvitoPluginPayload(logger, page)
        );
        Result scrollingTopResult = await context.ExecutePlugin(scrollTopCommand);
        Assert.True(scrollingTopResult.IsSuccess);

        QuerySelectorPayload querySelector = new QuerySelectorPayload(
            "popular-rubricator-button-DK2W3"
        );
        PluginCommand getElementCommand = new PluginCommand(
            "GetElementPlugin",
            new AvitoPluginPayload(logger, page, querySelector)
        );

        Result<IElementHandle> elementHandleResult = await context.ExecutePlugin<IElementHandle>(
            getElementCommand
        );
        Assert.True(elementHandleResult.IsSuccess);

        await using IElementHandle elementHandle = elementHandleResult.Value;

        try
        {
            PluginCommand clickCommand = new PluginCommand(
                "ClickElementPlugin",
                new AvitoPluginPayload(elementHandle, logger)
            );
            Result clicking = await context.ExecutePlugin(clickCommand);
            Assert.True(clicking.IsSuccess);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            logger.Fatal(
                "{Test} failed. Error: {Message}",
                nameof(Get_Element_Attribute_Test),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }
}
