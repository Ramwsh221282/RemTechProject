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
                new GetSingleElementBehavior(
                    pool,
                    filterInputPath,
                    filterInputPathType,
                    "filter-input"
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    "filter-input",
                    [element => new ClickOnElementBehavior(element)]
                )
            )
            .AddBehavior(
                new GetSingleElementBehavior(
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
                    factories: [element => new ClickOnElementBehavior(element).WithWait(5)]
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    parentName: "check-boxes",
                    factories:
                    [
                        element => new InitializeAttributeBehavior(element, checkedAttribute),
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
                new GetSingleElementBehavior(
                    pool,
                    filterInputPath,
                    filterInputPathType,
                    "filter-input"
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    "filter-input",
                    [element => new ClickOnElementBehavior(element)]
                )
            )
            .AddBehavior(
                new GetSingleElementBehavior(
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
                new GetSingleElementBehavior(
                    pool,
                    popularMarkButtonXpath,
                    pathType,
                    popularMarkButton
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    popularMarkButton,
                    element => new ClickOnElementBehavior(element)
                )
            )
            .AddBehavior(
                new GetSingleElementBehavior(
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
                    element => new ExcludeChildsBehavior(element, child => child.Text != param)
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    popularMarksRubricatorName,
                    element => new ClickOnElementBehavior(element)
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
                new StartBehavior("none"),
                new OpenPageBehavior(avitoUrl),
                new ScrollToBottomBehavior(),
                new ScrollToTopBehavior()
            )
            .AddBehavior(
                new GetSingleElementBehavior(
                    pool,
                    popularMarkButtonXpath,
                    pathType,
                    popularMarkButton
                )
            )
            .AddBehavior(
                new DoForExactParent(
                    pool,
                    popularMarkButton,
                    element => new ClickOnElementBehavior(element)
                )
            )
            .AddBehavior(
                new GetSingleElementBehavior(
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
                    element => new ExcludeChildsBehavior(
                        element,
                        child => parameters.All(param => child.Text != param)
                    )
                )
            )
            .AddBehavior(
                new DoForAllChildren(
                    pool,
                    popularMarksRubricatorName,
                    element => new SetAvitoMarkFiltersBehavior(element)
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
            .AddBehavior(new GetSingleElementBehavior(pool, stateAllXpath, pathType, stateAll))
            .AddBehavior(new GetSingleElementBehavior(pool, stateNewXpath, pathType, stateNew))
            .AddBehavior(new GetSingleElementBehavior(pool, stateBUXpath, pathType, stateBU))
            .AddBehavior(
                new DoForSpecificParents(
                    pool,
                    el => el.Name == param,
                    el => new ClickOnElementBehavior(el)
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
            .AddBehavior(new GetSingleElementBehavior(pool, ndsXpath, pathType, nds))
            .AddBehavior(new GetSingleElementBehavior(pool, lizingXpath, pathType, lizing))
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
            .AddBehavior(new GetSingleElementBehavior(pool, containerPath, pathType, containerName))
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
}
