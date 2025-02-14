using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Tests.FeaturesTDD.CatalogueParsers;

public sealed class CatalogueParsersTests
{
    private const string host = "localhost";
    private const string user = "rmuser";
    private const string password = "rmpassword";
    private const string queue = "web-driver-service";
    private const string url =
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    public CatalogueParsersTests()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddLogging();
        collection.InitializeWorkerDependencies(queue, host, user, password);
        collection.AddSingleton<Worker>();
        _serviceProvider = collection.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger>();
    }

    [Fact]
    public async Task Test_Parse_Pagination()
    {
        const string pathType = "xpath";
        const string paginationPath = ".//ul[@data-marker='pagination-button']";
        const string pagination = "pagination";

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        WebElementPool pool = new();
        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(url),
                new ScrollToBottomRetriable(10),
                new ScrollToTopRetriable(10)
            )
            .AddBehavior(new GetNewElementRetriable(pool, paginationPath, pathType, pagination, 10))
            .AddBehavior(new DoForLastParent(pool, element => new InitializeTextBehavior(element)))
            .AddOnFinish(new StopBehavior());

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        using WebDriverSession session = new(publisher);
        await session.ExecuteBehavior(pipeLine, ct);

        await worker.StopAsync(ct);

        Result<WebElement> paginationElement = pool[pool.Count - 1];
        Assert.True(paginationElement.IsSuccess);

        Assert.NotEqual(string.Empty, paginationElement.Value.Text);
        _logger.Information("Pagination data: {Text}", paginationElement.Value.Text);

        // post processing part
        string text = paginationElement.Value.Text;
        ReadOnlySpan<char> before = text;
        int lastOfRN = before.LastIndexOf('\n');
        ReadOnlySpan<char> after = before.Slice(lastOfRN + 1);
        bool canParse = int.TryParse(after, out int pageNumber);
        Assert.True(canParse);
        _logger.Information("Page number of last page: {pageNumber}", pageNumber);
    }

    [Fact]
    public async Task Parse_Catalogue_Single_Page()
    {
        const string url =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

        const string pathType = "xpath";
        const string cataloguePath = ".//div[@data-marker='catalog-serp']";
        const string catalogue = "catalogue";

        const string itemPath = ".//div[@data-marker='item']";
        const string item = "item";

        const string itemLinkPath = ".//a[@itemprop='url']";
        const string link = "link";
        const string linkAttribute = "href";

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        WebElementPool pool = new();
        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(url),
                new ScrollToBottomRetriable(10),
                new ScrollToTopRetriable(10)
            )
            .AddBehavior(new GetNewElementRetriable(pool, cataloguePath, pathType, catalogue, 10))
            .AddBehavior(
                new DoForLastParent(
                    pool,
                    element => new GetChildrenBehavior(element, item, itemPath, pathType)
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    catalogue,
                    element => new GetChildrenBehavior(element, link, itemLinkPath, linkAttribute)
                )
            );

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        using WebDriverSession session = new(publisher);
        await session.ExecuteBehavior(pipeLine, ct);

        Result<WebElement> container = pool[pool.Count - 1];
        Assert.True(container.IsSuccess);

        foreach (var child in container.Value.Childs)
            await session.ExecuteBehavior(
                new GetSingleChildAsParent(child, pool, itemLinkPath, pathType, "link-item")
            );

        for (int index = 1; index < pool.Count; index++)
            await session.ExecuteBehavior(
                new InitializeAttributeRepeatable(pool[index], linkAttribute, 10)
            );

        for (int index = 1; index < pool.Count; index++)
        {
            Result<WebElement> element = pool[index];
            Assert.True(element.IsSuccess);
            Assert.NotEqual(string.Empty, element.Value.Attributes[linkAttribute]);
            _logger.Information(
                "Advertisement link: {Link}",
                element.Value.Attributes[linkAttribute]
            );
        }

        await session.ExecuteBehavior(new StopBehavior());
        await worker.StopAsync(ct);

        int bpoint = 0;
    }
}
