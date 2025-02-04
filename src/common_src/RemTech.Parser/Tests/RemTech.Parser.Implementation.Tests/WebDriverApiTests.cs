using Microsoft.Extensions.DependencyInjection;
using RemTech.Parser.Contracts.Contracts;
using RemTech.Parser.Contracts.Contracts.Commands;
using RemTech.Parser.Contracts.Contracts.Queries;
using RemTech.Parser.Implementation.Injection;
using RemTechCommon.Injections;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Implementation.Tests;

public sealed class WebDriverApiTests
{
    private readonly IWebDriverApi _api;

    public WebDriverApiTests()
    {
        IServiceCollection services = new ServiceCollection();
        services = services.AddCommonInjections();
        services = new WebDriverParserPluginInjection().Inject(services);
        IServiceProvider provider = services.BuildServiceProvider();
        _api = provider.GetRequiredService<IWebDriverApi>();
    }

    [Fact]
    public async Task Open_WebDriver_Page_Test()
    {
        Result starting = await _api.ExecuteCommand(new StartWebDriverCommand());
        Assert.True(starting.IsSuccess);

        Result opening = await _api.ExecuteCommand(
            new OpenPageCommand(
                "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0"
            )
        );
        Assert.True(opening.IsSuccess);

        Result stopping = await _api.ExecuteCommand(new StopWebDriverCommand());
        Assert.True(stopping.IsSuccess);
    }

    [Fact]
    public async Task Open_WebDriver_Page_And_Scroll_Down_And_Bottom_Test()
    {
        Result starting = await _api.ExecuteCommand(new StartWebDriverCommand());
        Assert.True(starting.IsSuccess);

        Result opening = await _api.ExecuteCommand(
            new OpenPageCommand(
                "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0"
            )
        );
        Assert.True(opening.IsSuccess);

        Result bottomScrolling = await _api.ExecuteCommand(new ScrollToDownCommand());
        Assert.True(bottomScrolling.IsSuccess);

        Result topScrolling = await _api.ExecuteCommand(new ScrollToTopCommand());
        Assert.True(topScrolling.IsSuccess);

        Result stopping = await _api.ExecuteCommand(new StopWebDriverCommand());
        Assert.True(stopping.IsSuccess);
    }

    [Fact]
    public async Task Test_Avito_Get_Transport_Types_Filter_Input_Container()
    {
        Result starting = await _api.ExecuteCommand(new StartWebDriverCommand());
        Assert.True(starting.IsSuccess);

        Result opening = await _api.ExecuteCommand(
            new OpenPageCommand(
                "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0"
            )
        );
        Assert.True(opening.IsSuccess);

        Result bottomScrolling = await _api.ExecuteCommand(new ScrollToDownCommand());
        Assert.True(bottomScrolling.IsSuccess);

        Result topScrolling = await _api.ExecuteCommand(new ScrollToTopCommand());
        Assert.True(topScrolling.IsSuccess);

        const string path = ".//div[@class='form-mainFilters-y0xZT']";
        GetElementByXPathQuery query = new GetElementByXPathQuery(path);

        Result<WebElementObject> element = await _api.ExecuteQuery<
            GetElementQuery,
            WebElementObject
        >(query);

        Assert.True(element.IsSuccess);
        Assert.Equal(0, element.Value.Position);

        Result stopping = await _api.ExecuteCommand(new StopWebDriverCommand());
        Assert.True(stopping.IsSuccess);
    }

    [Fact]
    public async Task FindElementInsideOfExistingElement()
    {
        Result starting = await _api.ExecuteCommand(new StartWebDriverCommand());
        Assert.True(starting.IsSuccess);

        Result opening = await _api.ExecuteCommand(
            new OpenPageCommand(
                "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0"
            )
        );
        Assert.True(opening.IsSuccess);

        Result bottomScrolling = await _api.ExecuteCommand(new ScrollToDownCommand());
        Assert.True(bottomScrolling.IsSuccess);

        Result topScrolling = await _api.ExecuteCommand(new ScrollToTopCommand());
        Assert.True(topScrolling.IsSuccess);

        const string path = ".//div[@class='form-mainFilters-y0xZT']";
        GetElementByXPathQuery query_1 = new GetElementByXPathQuery(path);

        Result<WebElementObject> element_1 = await _api.ExecuteQuery<
            GetElementQuery,
            WebElementObject
        >(query_1);

        Assert.True(element_1.IsSuccess);
        Assert.Equal(0, element_1.Value.Position);

        const string tag = "form";
        GetElementInsideOfElementQuery query_2 = new GetElementInsideOfElementQuery(
            new GetElementByTagQuery(tag),
            element_1
        );

        Result<WebElementObject> element_2 = await _api.ExecuteQuery<
            GetElementInsideOfElementQuery,
            WebElementObject
        >(query_2);
        Assert.True(element_2.IsSuccess);
        Assert.Equal(1, element_2.Value.Position);

        const string path_2 =
            ".//div[@class='styles-module-col-F4VNN styles-module-col_span_12-bhIkA']";
        GetElementInsideOfElementQuery query_3 = new GetElementInsideOfElementQuery(
            new GetElementByXPathQuery(path_2),
            element_2
        );

        Result<WebElementObject> element_3 = await _api.ExecuteQuery<
            GetElementInsideOfElementQuery,
            WebElementObject
        >(query_3);

        Assert.True(element_3.IsSuccess);
        Assert.Equal(2, element_3.Value.Position);

        Result stopping = await _api.ExecuteCommand(new StopWebDriverCommand());
        Assert.True(stopping.IsSuccess);
    }

    [Fact]
    public async Task Find_Multiple_Elements_SameXPath_In_One_Element()
    {
        Result starting = await _api.ExecuteCommand(new StartWebDriverCommand());
        Assert.True(starting.IsSuccess);

        Result opening = await _api.ExecuteCommand(
            new OpenPageCommand(
                "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0"
            )
        );
        Assert.True(opening.IsSuccess);

        Result bottomScrolling = await _api.ExecuteCommand(new ScrollToDownCommand());
        Assert.True(bottomScrolling.IsSuccess);

        Result topScrolling = await _api.ExecuteCommand(new ScrollToTopCommand());
        Assert.True(topScrolling.IsSuccess);

        const string path = ".//div[@class='form-mainFilters-y0xZT']";
        GetElementByXPathQuery query_1 = new GetElementByXPathQuery(path);

        Result<WebElementObject> element_1 = await _api.ExecuteQuery<
            GetElementQuery,
            WebElementObject
        >(query_1);

        Assert.True(element_1.IsSuccess);
        Assert.Equal(0, element_1.Value.Position);

        const string path_2 =
            ".//div[@class='styles-module-root-G07MD styles-module-root_dense-kUp8z styles-module-root_compensate_bottom-WEqOQ']";
        GetElementsInsideOfElementQuery query_2 = new GetElementsInsideOfElementQuery(
            element_1,
            new GetElementByXPathQuery(path_2)
        );
        Result<WebElementObject[]> elements = await _api.ExecuteQuery<
            GetElementsInsideOfElementQuery,
            WebElementObject[]
        >(query_2);
        Assert.True(elements.IsSuccess);

        for (int index = 0; index < elements.Value.Length; index++)
        {
            int position = index + 1;
            Assert.Equal(position, elements.Value[index].Position);
        }

        Result stopping = await _api.ExecuteCommand(new StopWebDriverCommand());
        Assert.True(stopping.IsSuccess);
    }
}
