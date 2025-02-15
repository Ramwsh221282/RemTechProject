using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Tests.FeaturesTDD.TransportTypesFeature.CustomBehaviors;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Tests.FeaturesTDD.TransportTypesFeature;

public sealed class RefactoredTransportTypesFeatureTest
{
    private const string host = "localhost";
    private const string user = "rmuser";
    private const string password = "rmpassword";
    private const string queue = "web-driver-service";
    private const string avitoUrl =
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    public RefactoredTransportTypesFeatureTest()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddLogging();
        collection.InitializeWorkerDependencies(queue, host, user, password);
        collection.AddSingleton<Worker>();
        _serviceProvider = collection.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger>();
    }

    [Fact]
    public async Task Test_TransportTypes_Feature_Refactored()
    {
        string[] parameters =
        [
            "Боковой погрузчик",
            "Вилочный погрузчик",
            "Мини-думпер",
            "Портальный погрузчик",
        ];

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        WebElementPool pool = new();

        const string filterInputPath =
            ".//input[@data-marker='params[111024]/multiselect-outline-input/input']";
        const string filterInputPathType = "xpath";
        const string checkBoxContainerPath = ".//div[@data-marker='params[111024]/list']";
        const string checkBoxPath = ".//label[@role='checkbox']";
        const string checkedAttribute = "aria-checked";

        using WebDriverSession session = new(publisher);
        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(avitoUrl).WithWait(5),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(
                new GetNewElementInstant(pool, filterInputPath, filterInputPathType, "filter-input")
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    "filter-input",
                    [element => new ClickOnElementInstant(element)]
                )
            )
            .AddBehavior(
                new GetNewElementInstant(
                    pool,
                    checkBoxContainerPath,
                    filterInputPathType,
                    "check-boxes"
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    "check-boxes",
                    [
                        element => new GetChildrenBehavior(
                            element,
                            "check-boxes",
                            checkBoxPath,
                            filterInputPathType
                        ),
                    ]
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    parentName: "check-boxes",
                    factories: [element => new InitializeTextBehavior(element)]
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    parentName: "check-boxes",
                    behaviorFactories:
                    [
                        element => new ExcludeChildsBehavior(
                            element,
                            child => parameters.All(param => child.Text != param)
                        ),
                    ]
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    parentName: "check-boxes",
                    factories: [element => new ClickOnElementInstant(element)]
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    parentName: "check-boxes",
                    factories:
                    [
                        element => new InitializeAttributeInstant(element, checkedAttribute),
                    ]
                )
            );

        IWebDriverBehavior behavior = pipeLine;
        Result processing = await session.ExecuteBehavior(behavior, ct);
        Assert.True(processing.IsSuccess);

        await publisher.Send(new StopWebDriverContract(), ct);
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Do_Set_Filters_With_Custom_Behavior()
    {
        string[] parameters =
        [
            "Боковой погрузчик",
            "Вилочный погрузчик",
            "Мини-думпер",
            "Портальный погрузчик",
        ];

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        WebElementPool pool = new();

        const string filterInputPath =
            ".//input[@data-marker='params[111024]/multiselect-outline-input/input']";
        const string filterInputPathType = "xpath";
        const string checkBoxContainerPath = ".//div[@data-marker='params[111024]/list']";
        const string checkBoxPath = ".//label[@role='checkbox']";

        using WebDriverSession session = new(publisher);
        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(avitoUrl),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(
                new GetNewElementInstant(pool, filterInputPath, filterInputPathType, "filter-input")
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    "filter-input",
                    [element => new ClickOnElementInstant(element)]
                )
            )
            .AddBehavior(
                new GetNewElementInstant(
                    pool,
                    checkBoxContainerPath,
                    filterInputPathType,
                    "check-boxes"
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    "check-boxes",
                    [
                        element => new GetChildrenBehavior(
                            element,
                            "check-boxes",
                            checkBoxPath,
                            filterInputPathType
                        ),
                    ]
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    parentName: "check-boxes",
                    factories: [element => new InitializeTextBehavior(element)]
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    parentName: "check-boxes",
                    behaviorFactories: element => new SetAvitoTransportTypeFiltersBehavior(
                        element,
                        parameters
                    )
                )
            );

        IWebDriverBehavior behavior = pipeLine;
        Result processing = await session.ExecuteBehavior(behavior, ct);
        Assert.True(processing.IsSuccess);

        await publisher.Send(new StopWebDriverContract(), ct);
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Get_Mark_Filters()
    {
        const string pathType = "xpath";

        const string param = "LuGong";

        const string popularMarkButtonXpath =
            ".//button[@data-marker='popular-rubricator/controls/all']";
        const string popularMarkButton = "popular-marks-button";

        const string popularMarksRubricatorContainerXPath =
            ".//div[@class='popular-rubricator-links-o9b47']";
        const string popularMarksRubricatorName = "popular-marks-container";

        const string popularMarkRubricatorLinkXPath =
            ".//a[@data-marker='popular-rubricator/link']";
        const string popularMarkRubricatorName = "popular-mark-rubricator";
        const string hrefAttribute = "href";

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        WebElementPool pool = new();
        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(avitoUrl),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(
                new GetNewElementInstant(pool, popularMarkButtonXpath, pathType, popularMarkButton)
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    popularMarkButton,
                    element => new ClickOnElementInstant(element)
                )
            )
            .AddBehavior(
                new GetNewElementInstant(
                    pool,
                    popularMarksRubricatorContainerXPath,
                    pathType,
                    popularMarksRubricatorName
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    popularMarksRubricatorName,
                    element => new GetChildrenBehavior(
                        element,
                        popularMarkRubricatorName,
                        popularMarkRubricatorLinkXPath,
                        pathType
                    )
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    popularMarksRubricatorName,
                    element => new InitializeTextBehavior(element)
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    popularMarksRubricatorName,
                    element => new InitializeAttributeRepeatable(element, hrefAttribute, 10)
                )
            );

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using WebDriverSession session = new(publisher);
        await worker.StartAsync(ct);
        await session.ExecuteBehavior(pipeLine, ct);
        await publisher.Send(new StopWebDriverContract(), ct);
        await worker.StopAsync(ct);

        Result<WebElement> rubricatorLinksParent = pool[pool.Count - 1];
        Assert.True(rubricatorLinksParent.IsSuccess);
        foreach (var child in rubricatorLinksParent.Value.Childs)
        {
            Assert.True(child.Attributes.ContainsKey(hrefAttribute));
            Assert.NotEqual(string.Empty, child.Attributes[hrefAttribute]);
            _logger.Information("Mark href: {Href}", child.Attributes[hrefAttribute]);
        }
    }

    [Fact]
    public async Task Marker_Filters_Interaction_With_Parameter()
    {
        const string pathType = "xpath";

        string[] parameters = ["LuGong", "BIZON", "Forward"];

        const string popularMarkButtonXpath =
            ".//button[@data-marker='popular-rubricator/controls/all']";
        const string popularMarkButton = "popular-marks-button";

        const string popularMarksRubricatorContainerXPath =
            ".//div[@class='popular-rubricator-links-o9b47']";
        const string popularMarksRubricatorName = "popular-marks-container";

        const string popularMarkRubricatorLinkXPath =
            ".//a[@data-marker='popular-rubricator/link']";
        const string popularMarkRubricatorName = "popular-mark-rubricator";

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        WebElementPool pool = new();
        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new CompositeBehavior()
                    .AddBehavior(new StartBehavior("none"))
                    .AddBehavior(new OpenPageBehavior(avitoUrl))
                    .AddBehavior(new ScrollToBottomBehavior())
                    .AddBehavior(new ScrollToTopBehavior())
            )
            .AddBehavior(
                new GetNewElementInstant(pool, popularMarkButtonXpath, pathType, popularMarkButton)
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    popularMarkButton,
                    element => new ClickOnElementInstant(element)
                )
            )
            .AddBehavior(
                new GetNewElementInstant(
                    pool,
                    popularMarksRubricatorContainerXPath,
                    pathType,
                    popularMarksRubricatorName
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    popularMarksRubricatorName,
                    element => new GetChildrenBehavior(
                        element,
                        popularMarkRubricatorName,
                        popularMarkRubricatorLinkXPath,
                        pathType
                    )
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    popularMarksRubricatorName,
                    element => new InitializeTextBehavior(element)
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    popularMarksRubricatorName,
                    _ => new DoForSpecificChildren(
                        pool,
                        popularMarksRubricatorName,
                        child => parameters.Any(param => param == child.Text),
                        child => new SetAvitoMarkFiltersBehavior(child)
                    )
                )
            )
            .AddBehavior(new ClearPoolBehavior(), new ClearClientPoolBehavior(pool));

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using WebDriverSession session = new(publisher);
        await worker.StartAsync(ct);
        Result result = await session.ExecuteBehavior(pipeLine, ct);
        Assert.True(result.IsSuccess);
        await publisher.Send(new StopWebDriverContract(), ct);
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task State_Filters_Parsing_Test()
    {
        const string pathType = "xpath";

        const string stateAllXpath = ".//label[@data-marker='params[110276]/all']";
        const string stateAll = "Все";

        const string stateNewXpath = ".//label[@data-marker='params[110276]/426646']";
        const string stateNew = "Новые";

        const string stateBUXpath = ".//label[@data-marker='params[110276]/426647']";
        const string stateBU = "Б/у";

        const string param = "Новые";

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        WebElementPool pool = new();

        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(avitoUrl).WithWait(10),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(new GetNewElementInstant(pool, stateAllXpath, pathType, stateAll))
            .AddBehavior(new GetNewElementInstant(pool, stateNewXpath, pathType, stateNew))
            .AddBehavior(new GetNewElementInstant(pool, stateBUXpath, pathType, stateBU))
            .AddBehavior(
                new DoForSpecificParents(
                    pool,
                    el => el.Name == param,
                    el => new ClickOnElementInstant(el)
                )
            );

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using WebDriverSession session = new(publisher);
        await worker.StartAsync(ct);
        Result result = await session.ExecuteBehavior(pipeLine, ct);
        Assert.True(result.IsSuccess);
        await publisher.Send(new StopWebDriverContract(), ct);
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Lizing_Or_Nds_Filter_Test()
    {
        const string param = "Можно в лизинг";

        const string pathType = "xpath";

        const string ndsXpath = ".//label[@data-marker='params[118803]/checkbox/1']";
        const string nds = "Продажа с НДС";

        const string lizingXpath = ".//label[@data-marker='params[124676]/checkbox/1']";
        const string lizing = "Можно в лизинг";

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        WebElementPool pool = new();

        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(avitoUrl).WithWait(10),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(new GetNewElementInstant(pool, ndsXpath, pathType, nds))
            .AddBehavior(new GetNewElementInstant(pool, lizingXpath, pathType, lizing))
            .AddBehavior(new ScrollToBottomBehavior())
            .AddBehavior(
                new DoForSpecificParents(
                    pool,
                    el => el.Name == param,
                    el => new SetLizingNdsFilterBehavior(el)
                )
            );

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using WebDriverSession session = new(publisher);
        await worker.StartAsync(ct);
        Result result = await session.ExecuteBehavior(pipeLine, ct);
        Assert.True(result.IsSuccess);
        await publisher.Send(new StopWebDriverContract(), ct);
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Customers_Radio_Buttons_Test()
    {
        const string param = "Компании";

        const string pathType = "xpath";

        const string containerPath = ".//div[@data-marker='user' and @role='group']";
        const string containerName = "customers-type";

        const string radioButtonPath = ".//label";
        const string radioButton = "radio-button";

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        WebElementPool pool = new();

        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(avitoUrl).WithWait(10),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(new GetNewElementInstant(pool, containerPath, pathType, containerName))
            .AddBehavior(new ScrollToBottomBehavior())
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    containerName,
                    element => new GetChildrenBehavior(
                        element,
                        radioButton,
                        radioButtonPath,
                        pathType
                    )
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    containerName,
                    element => new InitializeTextBehavior(element)
                )
            )
            .AddBehavior(
                new DoForSpecificChildren(
                    pool,
                    containerName,
                    el => el.Text == param,
                    el => new SetCustomerTypeBehaviour(el)
                )
            );

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using WebDriverSession session = new(publisher);
        await worker.StartAsync(ct);
        Result result = await session.ExecuteBehavior(pipeLine, ct);
        Assert.True(result.IsSuccess);
        await publisher.Send(new StopWebDriverContract(), ct);
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Rating_And_Official_Dealer_Test()
    {
        const string param = "Официальный дилер";

        const string pathType = "xpath";

        const string ratingFourStarsAndMorePath =
            ".//label[@data-marker='params[115385]/checkbox/1212562']";
        const string ratingFourStarsAndMore = "four-stars";

        const string ratingCompaniesPath = ".//label[@data-marker='params[172422]/checkbox/1']";
        const string ratingCompanies = "companies";

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        WebElementPool pool = new();

        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(avitoUrl).WithWait(10),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(
                new GetNewElementInstant(
                    pool,
                    ratingFourStarsAndMorePath,
                    pathType,
                    ratingFourStarsAndMore
                )
            )
            .AddBehavior(
                new GetNewElementInstant(pool, ratingCompaniesPath, pathType, ratingCompanies)
            )
            .AddBehavior(new ScrollToBottomBehavior())
            .AddBehavior(new DoForAllParents(pool, element => new InitializeTextBehavior(element)))
            .AddBehavior(
                new DoForSpecificParents(
                    pool,
                    el => el.Text == param,
                    el => new SetRatingDealerFilterBehavior(el)
                )
            );

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using WebDriverSession session = new(publisher);
        await worker.StartAsync(ct);
        Result result = await session.ExecuteBehavior(pipeLine, ct);
        Assert.True(result.IsSuccess);
        await publisher.Send(new StopWebDriverContract(), ct);
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Submit_Filters_Button_Click()
    {
        const string pathType = "xpath";
        const string path =
            ".//button[@type='button' and @data-marker='search-filters/submit-button']";
        const string name = "submit-filter-button";

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        WebElementPool pool = new();

        CompositeBehavior pipeLine = new CompositeBehavior(_logger)
            .AddBehavior(
                new StartBehavior("none"),
                new OpenPageBehavior(avitoUrl).WithWait(10),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(new GetNewElementInstant(pool, path, pathType, name))
            .AddBehavior(new ScrollToBottomBehavior())
            .AddBehavior(new DoForExactParent(pool, name, el => new SetFiltersBehavior(el)));

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        using Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using WebDriverSession session = new(publisher);
        await worker.StartAsync(ct);
        Result result = await session.ExecuteBehavior(pipeLine, ct);
        Assert.True(result.IsSuccess);
        await publisher.Send(new StopWebDriverContract(), ct);
        await worker.StopAsync(ct);
    }
}
