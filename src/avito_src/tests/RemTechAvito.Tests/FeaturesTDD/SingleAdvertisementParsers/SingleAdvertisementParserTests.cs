using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Tests.FeaturesTDD.SingleAdvertisementParsers;

public sealed class SingleAdvertisementParserTests
{
    private const string host = "localhost";
    private const string user = "rmuser";
    private const string password = "rmpassword";
    private const string queue = "web-driver-service";

    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    public SingleAdvertisementParserTests()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddLogging();
        collection.InitializeWorkerDependencies(queue, host, user, password);
        collection.AddSingleton<Worker>();
        _serviceProvider = collection.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger>();
    }

    [Fact]
    public async Task Parse_Title_One_Page()
    {
        const string adUrl =
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA";
        const string pathType = "xpath";
        const string titlePath = ".//h1[@data-marker='item-view/title-info']";
        const string title = "title";

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        WebElementPool pool = new();
        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(adUrl),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(new GetNewElementInstant(pool, titlePath, pathType, title))
            .AddBehavior(new DoForAllParents(pool, element => new InitializeTextBehavior(element)))
            .AddBehavior(new StopBehavior());

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        using WebDriverSession session = new(publisher);
        Result execution = await session.ExecuteBehavior(pipeLine, ct);
        Assert.True(execution.IsSuccess);

        Result<WebElement> titleElement = pool[0];
        Assert.True(titleElement.IsSuccess);
        Assert.NotEqual(string.Empty, titleElement.Value.Text);
        _logger.Information("Title: {Title}", titleElement.Value.Text);
        pool.Clear();
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Parse_Title_Multiple_Page()
    {
        string[] adUrls =
        [
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/nalchik/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_lonking_fd15t_2025_2679224027?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/sankt-peterburg/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_yingfa_yfc35a_2025_4636590314?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds60-i_2024_4709685495?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
        ];

        const string pathType = "xpath";
        const string titlePath = ".//h1[@data-marker='item-view/title-info']";
        const string title = "title";

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using WebDriverSession session = new(publisher);

        Result opening = await session.ExecuteBehavior(new StartBehavior("none"));
        Assert.True(opening.IsSuccess);

        WebElementPool pool = new();

        CompositeBehavior pipeLine = new();

        foreach (var url in adUrls)
        {
            pipeLine = pipeLine
                .AddBehavior(new OpenPageBehavior(url))
                .AddBehavior(new ScrollToBottomRetriable(10))
                .AddBehavior(new ScrollToTopRetriable(10))
                .AddBehavior(new GetNewElementRetriable(pool, titlePath, pathType, title, 10))
                .AddBehavior(
                    new DoForLastParent(pool, element => new InitializeTextBehavior(element))
                )
                .AddOnFinish(new StopBehavior());
        }

        await session.ExecuteBehavior(pipeLine);

        IEnumerable<WebElement> elements = pool.Elements;

        foreach (var element in elements)
        {
            Assert.NotEqual(string.Empty, element.Text);
            _logger.Information("Title: {Text}", element.Text);
        }

        pool.Clear();
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Parse_Price_Single_Page()
    {
        const string adUrl =
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA";
        const string pathType = "xpath";

        const string pricePath = ".//span[@data-marker='item-view/item-price']";
        const string price = "price-value";
        const string priceAttribute = "content";

        const string currencyPath = ".//span[@itemprop='priceCurrency']";
        const string currency = "currency";
        const string currencyAttribute = "content";

        const string extraPath = ".//span[@class='style-price-value-additional-pFInr']";
        const string extra = "price-extra";

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        WebElementPool pool = new();
        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(adUrl),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(new GetNewElementRetriable(pool, pricePath, pathType, price, 10))
            .AddBehavior(
                new DoForLastParent(
                    pool,
                    element => new InitializeAttributeRepeatable(element, priceAttribute, 10)
                )
            )
            .AddBehavior(new GetNewElementRetriable(pool, currencyPath, pathType, currency, 10))
            .AddBehavior(
                new DoForLastParent(
                    pool,
                    element => new InitializeAttributeRepeatable(element, currencyAttribute, 10)
                )
            )
            .AddBehavior(new GetNewElementRetriable(pool, extraPath, pathType, extra, 10))
            .AddBehavior(new DoForLastParent(pool, element => new InitializeTextBehavior(element)))
            .AddOnFinish(new StopBehavior());

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);
        using WebDriverSession session = new(publisher);
        await session.ExecuteBehavior(pipeLine, ct);
        await worker.StopAsync(ct);

        Result<WebElement> priceElement = pool[0];
        Result<WebElement> currencyElement = pool[1];
        Result<WebElement> extraElement = pool[2];
        Assert.True(priceElement.IsSuccess);
        Assert.True(currencyElement.IsSuccess);
        Assert.True(extraElement.IsSuccess);
        Assert.NotEqual(string.Empty, priceElement.Value.Attributes[priceAttribute]);
        Assert.NotEqual(string.Empty, currencyElement.Value.Attributes[currencyAttribute]);
        Assert.NotEqual(string.Empty, extraElement.Value.Text);
        _logger.Information("Price: {Text}", priceElement.Value.Attributes[priceAttribute]);
        _logger.Information(
            "Currency: {Text}",
            currencyElement.Value.Attributes[currencyAttribute]
        );
        _logger.Information("Extra: {Text}", extraElement.Value.Text);
        pool.Clear();
    }

    [Fact]
    public async Task Parse_Price_Multiple_Pages()
    {
        string[] adUrls =
        [
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/nalchik/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_lonking_fd15t_2025_2679224027?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/sankt-peterburg/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_yingfa_yfc35a_2025_4636590314?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds60-i_2024_4709685495?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
        ];

        const string pathType = "xpath";

        const string pricePath = ".//span[@data-marker='item-view/item-price']";
        const string price = "price-value";
        const string priceAttribute = "content";

        const string currencyPath = ".//span[@itemprop='priceCurrency']";
        const string currency = "currency";
        const string currencyAttribute = "content";

        const string extraPath = ".//span[@class='style-price-value-additional-pFInr']";
        const string extra = "price-extra";

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using WebDriverSession session = new(publisher);
        await session.ExecuteBehavior(new StartBehavior("none"));

        foreach (var url in adUrls)
        {
            WebElementPool pool = new();
            CompositeBehavior pipeLine = new CompositeBehavior()
                .AddBehavior(new OpenPageBehavior(url))
                .AddBehavior(new ScrollToBottomRetriable(10))
                .AddBehavior(new ScrollToTopRetriable(10))
                .AddBehavior(new GetNewElementRetriable(pool, pricePath, pathType, price, 10))
                .AddBehavior(
                    new DoForLastParent(
                        pool,
                        element => new InitializeAttributeRepeatable(element, priceAttribute, 10)
                    )
                )
                .AddBehavior(new GetNewElementRetriable(pool, currencyPath, pathType, currency, 10))
                .AddBehavior(
                    new DoForLastParent(
                        pool,
                        element => new InitializeAttributeRepeatable(element, currencyAttribute, 10)
                    )
                )
                .AddBehavior(new GetNewElementRetriable(pool, extraPath, pathType, extra, 10))
                .AddBehavior(
                    new DoForLastParent(pool, element => new InitializeTextBehavior(element))
                );

            await session.ExecuteBehavior(pipeLine, ct);

            Result<WebElement> priceElement = pool[0];
            Result<WebElement> currencyElement = pool[1];
            Result<WebElement> extraElement = pool[2];
            Assert.True(priceElement.IsSuccess);
            Assert.True(currencyElement.IsSuccess);
            Assert.True(extraElement.IsSuccess);
            Assert.NotEqual(string.Empty, priceElement.Value.Attributes[priceAttribute]);
            Assert.NotEqual(string.Empty, currencyElement.Value.Attributes[currencyAttribute]);
            Assert.NotEqual(string.Empty, extraElement.Value.Text);
            _logger.Information("Price: {Text}", priceElement.Value.Attributes[priceAttribute]);
            _logger.Information(
                "Currency: {Text}",
                currencyElement.Value.Attributes[currencyAttribute]
            );
            _logger.Information("Extra: {Text}", extraElement.Value.Text);
            pool.Clear();
        }

        await session.ExecuteBehavior(new StopBehavior());
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Parse_Seller_Info_Single_Page()
    {
        const string adUrl =
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA";

        const string pathType = "xpath";
        const string sellerInfoPath = ".//div[@data-marker='seller-info/name']";
        const string selletInfo = "seller-info";

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        WebElementPool pool = new();
        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(adUrl),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(new GetNewElementInstant(pool, sellerInfoPath, pathType, selletInfo))
            .AddBehavior(new DoForLastParent(pool, element => new InitializeTextBehavior(element)))
            .AddOnFinish(new StopBehavior());

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        using WebDriverSession session = new(publisher);
        Result execution = await session.ExecuteBehavior(pipeLine, ct);
        Assert.True(execution.IsSuccess);

        Result<WebElement> sellerInfoElement = pool[0];
        Assert.True(sellerInfoElement.IsSuccess);
        Assert.NotEqual(string.Empty, sellerInfoElement.Value.Text);
        _logger.Information("Seller info: {Info}", sellerInfoElement.Value.Text);
        pool.Clear();
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Parse_Seller_Info_Multiple_Page()
    {
        string[] adUrls =
        [
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/nalchik/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_lonking_fd15t_2025_2679224027?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/sankt-peterburg/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_yingfa_yfc35a_2025_4636590314?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds60-i_2024_4709685495?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
        ];

        const string pathType = "xpath";
        const string sellerInfoPath = ".//div[@data-marker='seller-info/name']";
        const string selletInfo = "seller-info";

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using WebDriverSession session = new WebDriverSession(publisher);
        await session.ExecuteBehavior(new StartBehavior("none"));
        WebElementPool pool = new();

        foreach (var url in adUrls)
        {
            CompositeBehavior pipeLine = new CompositeBehavior()
                .AddBehavior(new OpenPageBehavior(url))
                .AddBehavior(new ScrollToBottomRetriable(10))
                .AddBehavior(new ScrollToTopRetriable(10))
                .AddBehavior(
                    new GetNewElementRetriable(pool, sellerInfoPath, pathType, selletInfo, 10)
                )
                .AddBehavior(
                    new DoForLastParent(pool, element => new InitializeTextBehavior(element))
                );

            await session.ExecuteBehavior(pipeLine, ct);
            Result<WebElement> sellerInfoElement = pool[pool.Count - 1];
            Assert.True(sellerInfoElement.IsSuccess);
            Assert.NotEqual(string.Empty, sellerInfoElement.Value.Text);
            _logger.Information("Price: {Info}", sellerInfoElement.Value.Text);
        }

        pool.Clear();
        await session.ExecuteBehavior(new StopBehavior());
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Parse_Characteristics_Single_Page()
    {
        const string adUrl =
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA";
        const string pathType = "xpath";
        const string containerPath = ".//ul[@class='params-paramsList-_awNW']";
        const string container = "characteristics-container";

        const string characteristicPath = ".//li[@class='params-paramsList__item-_2Y2O']";
        const string characteristic = "characteristic";

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        WebElementPool pool = new WebElementPool();
        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using WebDriverSession session = new WebDriverSession(publisher);

        CompositeBehavior behavior = new CompositeBehavior(_logger)
            .AddBehavior(new StartBehavior("none"))
            .AddBehavior(new OpenPageBehavior(adUrl))
            .AddBehavior(new ScrollToBottomRetriable(10))
            .AddBehavior(new ScrollToTopRetriable(10))
            .AddBehavior(new GetNewElementRetriable(pool, containerPath, pathType, container, 10))
            .AddBehavior(
                new DoForLastParent(
                    pool,
                    element => new GetChildrenBehavior(
                        element,
                        characteristic,
                        characteristicPath,
                        pathType
                    )
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    container,
                    element => new InitializeTextBehavior(element)
                )
            )
            .AddOnFinish(new StopBehavior());

        await session.ExecuteBehavior(behavior);

        Result<WebElement> characteristicsElement = pool[pool.Count - 1];
        Assert.True(characteristicsElement.IsSuccess);

        foreach (var child in characteristicsElement.Value.Childs)
        {
            Assert.NotEqual(string.Empty, child.Text);
            _logger.Information("Characteristics: {Characteristics}", child.Text);
        }

        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Parse_Characteristics_Multiple_Pages()
    {
        string[] adUrls =
        [
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/nalchik/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_lonking_fd15t_2025_2679224027?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/sankt-peterburg/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_yingfa_yfc35a_2025_4636590314?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds60-i_2024_4709685495?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
        ];

        const string pathType = "xpath";
        const string containerPath = ".//ul[@class='params-paramsList-_awNW']";
        const string container = "characteristics-container";

        const string characteristicPath = ".//li[@class='params-paramsList__item-_2Y2O']";
        const string characteristic = "characteristic";

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using WebDriverSession session = new WebDriverSession(publisher);
        await session.ExecuteBehavior(new StartBehavior("none"));
        WebElementPool pool = new WebElementPool();

        foreach (var url in adUrls)
        {
            CompositeBehavior pipeLine = new CompositeBehavior()
                .AddBehavior(new OpenPageBehavior(url))
                .AddBehavior(new ScrollToBottomRetriable(20))
                .AddBehavior(new ScrollToTopRetriable(20))
                .AddBehavior(
                    new GetNewElementRetriable(pool, containerPath, pathType, container, 20)
                )
                .AddBehavior(
                    new DoForLastParent(
                        pool,
                        element => new GetChildrenBehavior(
                            element,
                            characteristic,
                            characteristicPath,
                            pathType
                        )
                    )
                );
            await session.ExecuteBehavior(pipeLine);

            Result<WebElement> parent = pool[pool.Count - 1];
            if (parent.IsFailure)
                continue;

            foreach (var child in parent.Value.Childs)
                await session.ExecuteBehavior(new InitializeTextRepeatable(child, 10));
        }

        foreach (var parent in pool.Elements)
        {
            foreach (var child in parent.Childs)
            {
                Assert.NotEqual(string.Empty, child.Text);
                _logger.Information("Characteristic: {Text}", child.Text);
            }
        }

        await session.ExecuteBehavior(new StopBehavior());
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Parse_Address_Single_Page_Test()
    {
        const string adUrl =
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA";
        const string pathType = "xpath";
        const string addressPath = ".//div[@itemprop='address']";
        const string address = "address";

        WebElementPool pool = new();
        CompositeBehavior behavior = new CompositeBehavior(_logger)
            .AddBehavior(new StartBehavior("none"))
            .AddBehavior(new OpenPageBehavior(adUrl))
            .AddBehavior(new ScrollToBottomRetriable(10))
            .AddBehavior(new ScrollToTopRetriable(10))
            .AddBehavior(new GetNewElementRetriable(pool, addressPath, pathType, address, 10))
            .AddBehavior(
                new DoForLastParent(pool, element => new InitializeTextRepeatable(element, 10))
            )
            .AddOnFinish(new StopBehavior());

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        using WebDriverSession session = new WebDriverSession(publisher);
        await session.ExecuteBehavior(behavior, ct);

        await worker.StopAsync(ct);

        Result<WebElement> element = pool[pool.Count - 1];
        Assert.True(element.IsSuccess);
        Assert.NotEqual(string.Empty, element.Value.Text);
        _logger.Information("Address: {Text}", element.Value.Text);
    }

    [Fact]
    public async Task Parse_Address_Multiple_Pages()
    {
        string[] adUrls =
        [
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/nalchik/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_lonking_fd15t_2025_2679224027?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/sankt-peterburg/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_yingfa_yfc35a_2025_4636590314?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds60-i_2024_4709685495?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
        ];

        const string pathType = "xpath";
        const string addressPath = ".//div[@itemprop='address']";
        const string address = "address";

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        using WebDriverSession session = new WebDriverSession(publisher);
        await session.ExecuteBehavior(new StartBehavior("none"));

        WebElementPool pool = new WebElementPool();
        foreach (var url in adUrls)
        {
            CompositeBehavior behavior = new CompositeBehavior(_logger)
                .AddBehavior(new OpenPageBehavior(url))
                .AddBehavior(new ScrollToBottomRetriable(10))
                .AddBehavior(new ScrollToTopRetriable(10))
                .AddBehavior(new GetNewElementRetriable(pool, addressPath, pathType, address, 10))
                .AddBehavior(
                    new DoForLastParent(pool, element => new InitializeTextRepeatable(element, 10))
                );

            await session.ExecuteBehavior(behavior, ct);
        }

        await session.ExecuteBehavior(new StopBehavior());
        await worker.StopAsync(ct);

        foreach (var parent in pool)
        {
            Assert.NotEqual(string.Empty, parent.Text);
            _logger.Information("Address: {Text}", parent.Text);
        }
    }

    [Fact]
    public async Task Parse_Description_Single_Pages()
    {
        const string adUrl =
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA";
        const string pathType = "xpath";
        const string descriptionPath = ".//div[@itemprop='description']";
        const string Description = "description";

        WebElementPool pool = new();
        CompositeBehavior behavior = new CompositeBehavior(_logger)
            .AddBehavior(new StartBehavior("none"))
            .AddBehavior(new OpenPageBehavior(adUrl))
            .AddBehavior(new ScrollToBottomRetriable(10))
            .AddBehavior(new ScrollToTopRetriable(10))
            .AddBehavior(
                new GetNewElementRetriable(pool, descriptionPath, pathType, Description, 10)
            )
            .AddBehavior(
                new DoForLastParent(pool, element => new InitializeTextRepeatable(element, 10))
            )
            .AddOnFinish(new StopBehavior());

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        using WebDriverSession session = new WebDriverSession(publisher);
        await session.ExecuteBehavior(behavior, ct);

        await worker.StopAsync(ct);

        Result<WebElement> element = pool[pool.Count - 1];
        Assert.True(element.IsSuccess);
        Assert.NotEqual(string.Empty, element.Value.Text);
        _logger.Information("Description: {Text}", element.Value.Text);
    }

    [Fact]
    public async Task Parse_Description_Multiple_Pages()
    {
        string[] adUrls =
        [
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/nalchik/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_lonking_fd15t_2025_2679224027?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/sankt-peterburg/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_yingfa_yfc35a_2025_4636590314?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds60-i_2024_4709685495?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
        ];

        const string pathType = "xpath";
        const string descriptionPath = ".//div[@itemprop='description']";
        const string Description = "description";

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        using WebDriverSession session = new WebDriverSession(publisher);
        await session.ExecuteBehavior(new StartBehavior("none"));

        WebElementPool pool = new WebElementPool();
        foreach (var url in adUrls)
        {
            CompositeBehavior behavior = new CompositeBehavior(_logger)
                .AddBehavior(new OpenPageBehavior(url))
                .AddBehavior(new ScrollToBottomRetriable(10))
                .AddBehavior(new ScrollToTopRetriable(10))
                .AddBehavior(
                    new GetNewElementRetriable(pool, descriptionPath, pathType, Description, 10)
                )
                .AddBehavior(
                    new DoForLastParent(pool, element => new InitializeTextRepeatable(element, 10))
                );

            await session.ExecuteBehavior(behavior, ct);
        }

        await session.ExecuteBehavior(new StopBehavior());
        await worker.StopAsync(ct);

        foreach (var parent in pool)
        {
            Assert.NotEqual(string.Empty, parent.Text);
            _logger.Information("Description: {Text}", parent.Text);
        }
    }

    [Fact]
    public async Task Parse_IВ_Single_Page()
    {
        const string adUrl =
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA";
        const string pathType = "xpath";
        const string idPath = ".//span[@data-marker='item-view/item-id']";
        const string id = "id";

        WebElementPool pool = new();
        CompositeBehavior behavior = new CompositeBehavior(_logger)
            .AddBehavior(new StartBehavior("none"))
            .AddBehavior(new OpenPageBehavior(adUrl))
            .AddBehavior(new ScrollToBottomRetriable(10))
            .AddBehavior(new ScrollToTopRetriable(10))
            .AddBehavior(new GetNewElementRetriable(pool, idPath, pathType, id, 10))
            .AddBehavior(
                new DoForLastParent(pool, element => new InitializeTextRepeatable(element, 10))
            )
            .AddOnFinish(new StopBehavior());

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        using WebDriverSession session = new WebDriverSession(publisher);
        await session.ExecuteBehavior(behavior, ct);

        await worker.StopAsync(ct);

        Result<WebElement> element = pool[pool.Count - 1];
        Assert.True(element.IsSuccess);
        Assert.NotEqual(string.Empty, element.Value.Text);
        _logger.Information("Id: {Text}", element.Value.Text);
    }

    [Fact]
    public async Task Parse_ID_Multiple_Page()
    {
        string[] adUrls =
        [
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/nalchik/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_lonking_fd15t_2025_2679224027?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/sankt-peterburg/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_yingfa_yfc35a_2025_4636590314?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds60-i_2024_4709685495?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
        ];

        const string pathType = "xpath";
        const string idPath = ".//span[@data-marker='item-view/item-id']";
        const string id = "id";

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        using WebDriverSession session = new WebDriverSession(publisher);
        await session.ExecuteBehavior(new StartBehavior("none"));

        WebElementPool pool = new WebElementPool();
        foreach (var url in adUrls)
        {
            CompositeBehavior behavior = new CompositeBehavior(_logger)
                .AddBehavior(new OpenPageBehavior(url))
                .AddBehavior(new ScrollToBottomRetriable(10))
                .AddBehavior(new ScrollToTopRetriable(10))
                .AddBehavior(new GetNewElementRetriable(pool, idPath, pathType, id, 10))
                .AddBehavior(
                    new DoForLastParent(pool, element => new InitializeTextRepeatable(element, 10))
                );

            await session.ExecuteBehavior(behavior, ct);
        }

        await session.ExecuteBehavior(new StopBehavior());
        await worker.StopAsync(ct);

        foreach (var parent in pool)
        {
            Assert.NotEqual(string.Empty, parent.Text);
            _logger.Information("Id: {Text}", parent.Text);
        }
    }

    [Fact]
    public async Task Parse_Date_Single_Page()
    {
        const string adUrl =
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA";
        const string pathType = "xpath";
        const string datePath = ".//span[@data-marker='item-view/item-date']";
        const string date = "date";

        WebElementPool pool = new();
        CompositeBehavior behavior = new CompositeBehavior(_logger)
            .AddBehavior(new StartBehavior("none"))
            .AddBehavior(new OpenPageBehavior(adUrl))
            .AddBehavior(new ScrollToBottomRetriable(10))
            .AddBehavior(new ScrollToTopRetriable(10))
            .AddBehavior(new GetNewElementRetriable(pool, datePath, pathType, date, 10))
            .AddBehavior(
                new DoForLastParent(pool, element => new InitializeTextRepeatable(element, 10))
            )
            .AddOnFinish(new StopBehavior());

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        using WebDriverSession session = new WebDriverSession(publisher);
        await session.ExecuteBehavior(behavior, ct);

        await worker.StopAsync(ct);

        Result<WebElement> element = pool[pool.Count - 1];
        Assert.True(element.IsSuccess);
        Assert.NotEqual(string.Empty, element.Value.Text);
        _logger.Information("Id: {Text}", element.Value.Text);
    }

    [Fact]
    public async Task Parse_Date_Multiple_Pages()
    {
        string[] adUrls =
        [
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/nalchik/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_lonking_fd15t_2025_2679224027?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/sankt-peterburg/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_yingfa_yfc35a_2025_4636590314?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/moskva/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds60-i_2024_4709685495?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
            "https://www.avito.ru/groznyy/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_zauberg_ds25-x_2024_4613594198?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiI3SFVjSTNtNGpTQ042TDBEIjt9O2IbIT8AAAA",
        ];

        const string pathType = "xpath";
        const string datePath = ".//span[@data-marker='item-view/item-date']";
        const string date = "date";

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        using WebDriverSession session = new WebDriverSession(publisher);
        await session.ExecuteBehavior(new StartBehavior("none"));

        WebElementPool pool = new WebElementPool();
        foreach (var url in adUrls)
        {
            CompositeBehavior behavior = new CompositeBehavior(_logger)
                .AddBehavior(new OpenPageBehavior(url))
                .AddBehavior(new ScrollToBottomRetriable(10))
                .AddBehavior(new ScrollToTopRetriable(10))
                .AddBehavior(new GetNewElementRetriable(pool, datePath, pathType, date, 10))
                .AddBehavior(
                    new DoForLastParent(pool, element => new InitializeTextRepeatable(element, 10))
                );

            await session.ExecuteBehavior(behavior, ct);
        }

        await session.ExecuteBehavior(new StopBehavior());
        await worker.StopAsync(ct);

        foreach (var parent in pool)
        {
            Assert.NotEqual(string.Empty, parent.Text);
            _logger.Information("Id: {Text}", parent.Text);
        }
    }
}
