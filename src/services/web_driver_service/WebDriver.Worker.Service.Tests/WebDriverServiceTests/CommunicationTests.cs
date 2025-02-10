using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using Serilog;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.ClickOnElement;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.GetMultipleChildren;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.GetSingleChildElement;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.GetSingleElement;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.GetTextFromElement;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.OpenWebDriverPage;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.ScrollPageDown;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.ScrollPageTop;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.StartWebDriver;
using WebDriver.Worker.Service.Tests.WebDriverServiceTests.TestContracts.StopWebDriver;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests;

public sealed class CommunicationTests
{
    private const string localhost = "localhost";
    private const string user = "rmuser";
    private const string password = "rmpassword";
    private const string queue = "web-driver-service";
    private const string avitoUrl =
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

    private readonly IServiceProvider _serviceProvider;

    public CommunicationTests()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddLogging();
        collection.InitializeWorkerDependencies(queue, localhost, user, password);
        collection.AddSingleton<Worker>();
        _serviceProvider = collection.BuildServiceProvider();
    }

    [Fact]
    public async Task Test_Serialization_And_Deserialization()
    {
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task workerLife = worker.StartAsync(cancellationTokenSource.Token);

        SingleCommunicationPublisher publisher = new SingleCommunicationPublisher(
            queue,
            localhost,
            user,
            password
        );

        ContractActionResult result_1 = await publisher.SendCommand(
            new StartWebDriverContract("none")
        );
        Assert.True(result_1.IsSuccess);
        StartWebDriverContractResponse response_1 =
            result_1.FromResult<StartWebDriverContractResponse>();
        Assert.True(response_1.IsStarted);

        ContractActionResult result_2 = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(result_2.IsSuccess);
        OpenWebDriverPageResponse response_2 = result_2.FromResult<OpenWebDriverPageResponse>();
        Assert.Equal(avitoUrl, response_2.OpenedUrl);

        ContractActionResult result_3 = await publisher.SendCommand(new StopWebDriverContract());
        Assert.True(result_3.IsSuccess);
        StopWebDriverContractResponse response =
            result_3.FromResult<StopWebDriverContractResponse>();
        Assert.True(response.IsStopped);

        result_1 = await publisher.SendCommand(new StartWebDriverContract("none"));
        Assert.True(result_1.IsSuccess);
        response_1 = result_1.FromResult<StartWebDriverContractResponse>();
        Assert.True(response_1.IsStarted);

        result_2 = await publisher.SendCommand(new OpenWebDriverPageContract(avitoUrl));
        Assert.True(result_2.IsSuccess);
        response_2 = result_2.FromResult<OpenWebDriverPageResponse>();
        Assert.Equal(avitoUrl, response_2.OpenedUrl);

        result_3 = await publisher.SendCommand(new StopWebDriverContract());
        Assert.True(result_3.IsSuccess);
        response = result_3.FromResult<StopWebDriverContractResponse>();
        Assert.True(response.IsStopped);

        await workerLife;
    }

    [Fact]
    public async Task Test_GetSingle_Web_Element_Test()
    {
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task workerLife = worker.StartAsync(cancellationTokenSource.Token);

        MultiCommunicationPublisher publisher = new MultiCommunicationPublisher(
            queue,
            localhost,
            user,
            password
        );

        ContractActionResult start_contract = await publisher.SendCommand(
            new StartWebDriverContract("none")
        );
        Assert.True(start_contract.IsSuccess);
        StartWebDriverContractResponse start_response =
            start_contract.FromResult<StartWebDriverContractResponse>();
        Assert.True(start_response.IsStarted);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        OpenWebDriverPageResponse open_page_response =
            open_page_contract.FromResult<OpenWebDriverPageResponse>();
        Assert.Equal(avitoUrl, open_page_response.OpenedUrl);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        ScrollPageDownContractResponse scroll_page_down_response =
            scroll_page_down_contract.FromResult<ScrollPageDownContractResponse>();
        Assert.True(scroll_page_down_response.IsScrolled);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        ScrollPageTopResponse scroll_page_top_response =
            scroll_page_top_contract.FromResult<ScrollPageTopResponse>();
        Assert.True(scroll_page_top_response.IsScrolled);

        string path_element_1 = ".//div[@class='form-mainFilters-y0xZT']";
        string path_type_element_1 = "xpath";
        ContractActionResult get_element_1 = await publisher.SendCommand(
            new GetSingleElementContract(path_element_1, path_type_element_1)
        );
        Assert.True(get_element_1.IsSuccess);
        WebElementResponse get_element_1_response = get_element_1.FromResult<WebElementResponse>();
        Assert.Equal(path_element_1, get_element_1_response.ElementPath);
        Assert.Equal(path_type_element_1, get_element_1_response.ElementPathType);
        Assert.True(get_element_1_response.ElementId != Guid.Empty);

        ContractActionResult stop_driver_contract = await publisher.SendCommand(
            new StopWebDriverContract()
        );
        Assert.True(stop_driver_contract.IsSuccess);
        StopWebDriverContractResponse stop_driver_response =
            stop_driver_contract.FromResult<StopWebDriverContractResponse>();
        Assert.True(stop_driver_response.IsStopped);

        publisher.Dispose();
        await workerLife;
    }

    [Fact]
    public async Task Test_Child_Element_Contract()
    {
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task workerLife = worker.StartAsync(cancellationTokenSource.Token);

        MultiCommunicationPublisher publisher = new MultiCommunicationPublisher(
            queue,
            localhost,
            user,
            password
        );

        ContractActionResult start_contract = await publisher.SendCommand(
            new StartWebDriverContract("none")
        );
        Assert.True(start_contract.IsSuccess);
        StartWebDriverContractResponse start_response =
            start_contract.FromResult<StartWebDriverContractResponse>();
        Assert.True(start_response.IsStarted);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        OpenWebDriverPageResponse open_page_response =
            open_page_contract.FromResult<OpenWebDriverPageResponse>();
        Assert.Equal(avitoUrl, open_page_response.OpenedUrl);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        ScrollPageDownContractResponse scroll_page_down_response =
            scroll_page_down_contract.FromResult<ScrollPageDownContractResponse>();
        Assert.True(scroll_page_down_response.IsScrolled);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        ScrollPageTopResponse scroll_page_top_response =
            scroll_page_top_contract.FromResult<ScrollPageTopResponse>();
        Assert.True(scroll_page_top_response.IsScrolled);

        string path_element_1 = ".//div[@class='form-mainFilters-y0xZT']";
        string path_type_element_1 = "xpath";
        ContractActionResult get_element_1 = await publisher.SendCommand(
            new GetSingleElementContract(path_element_1, path_type_element_1)
        );
        Assert.True(get_element_1.IsSuccess);
        WebElementResponse get_element_1_response = get_element_1.FromResult<WebElementResponse>();
        Assert.Equal(path_element_1, get_element_1_response.ElementPath);
        Assert.Equal(path_type_element_1, get_element_1_response.ElementPathType);
        Assert.True(get_element_1_response.ElementId != Guid.Empty);

        string path_element_2 = "form";
        string type_element_2 = "tag";
        ContractActionResult get_child_element_1 = await publisher.SendCommand(
            new GetSingleChildElementContract(
                get_element_1_response,
                path_element_2,
                type_element_2
            )
        );
        Assert.True(get_child_element_1.IsSuccess);
        WebElementResponse get_child_element_1_response =
            get_child_element_1.FromResult<WebElementResponse>();
        Assert.Equal(path_element_2, get_child_element_1_response.ElementPath);
        Assert.Equal(type_element_2, get_child_element_1_response.ElementPathType);
        Assert.True(get_child_element_1_response.ElementId != Guid.Empty);

        ContractActionResult stop_driver_contract = await publisher.SendCommand(
            new StopWebDriverContract()
        );
        Assert.True(stop_driver_contract.IsSuccess);
        StopWebDriverContractResponse stop_driver_response =
            stop_driver_contract.FromResult<StopWebDriverContractResponse>();
        Assert.True(stop_driver_response.IsStopped);

        publisher.Dispose();
        await workerLife;
    }

    [Fact]
    public async Task Get_Multiple_Child_Elements_InParent_Test()
    {
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task workerLife = worker.StartAsync(cancellationTokenSource.Token);

        MultiCommunicationPublisher publisher = new MultiCommunicationPublisher(
            queue,
            localhost,
            user,
            password
        );

        ContractActionResult start_contract = await publisher.SendCommand(
            new StartWebDriverContract("none")
        );
        Assert.True(start_contract.IsSuccess);
        StartWebDriverContractResponse start_response =
            start_contract.FromResult<StartWebDriverContractResponse>();
        Assert.True(start_response.IsStarted);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        OpenWebDriverPageResponse open_page_response =
            open_page_contract.FromResult<OpenWebDriverPageResponse>();
        Assert.Equal(avitoUrl, open_page_response.OpenedUrl);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        ScrollPageDownContractResponse scroll_page_down_response =
            scroll_page_down_contract.FromResult<ScrollPageDownContractResponse>();
        Assert.True(scroll_page_down_response.IsScrolled);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        ScrollPageTopResponse scroll_page_top_response =
            scroll_page_top_contract.FromResult<ScrollPageTopResponse>();
        Assert.True(scroll_page_top_response.IsScrolled);

        string path_element_1 = ".//div[@class='form-mainFilters-y0xZT']";
        string path_type_element_1 = "xpath";
        ContractActionResult get_element_1 = await publisher.SendCommand(
            new GetSingleElementContract(path_element_1, path_type_element_1)
        );
        Assert.True(get_element_1.IsSuccess);
        WebElementResponse get_element_1_response = get_element_1.FromResult<WebElementResponse>();
        Assert.Equal(path_element_1, get_element_1_response.ElementPath);
        Assert.Equal(path_type_element_1, get_element_1_response.ElementPathType);
        Assert.True(get_element_1_response.ElementId != Guid.Empty);

        string path_element_2 = "form";
        string type_element_2 = "tag";
        ContractActionResult get_child_element_1 = await publisher.SendCommand(
            new GetSingleChildElementContract(
                get_element_1_response,
                path_element_2,
                type_element_2
            )
        );
        Assert.True(get_child_element_1.IsSuccess);
        WebElementResponse get_child_element_1_response =
            get_child_element_1.FromResult<WebElementResponse>();
        Assert.Equal(path_element_2, get_child_element_1_response.ElementPath);
        Assert.Equal(type_element_2, get_child_element_1_response.ElementPathType);
        Assert.True(get_child_element_1_response.ElementId != Guid.Empty);

        string path_children =
            ".//div[@class='styles-module-root-G07MD styles-module-root_dense-kUp8z styles-module-root_compensate_bottom-WEqOQ']";
        string type_children = "xpath";
        ContractActionResult get_children = await publisher.SendCommand(
            new GetMultipleChildrenContract(
                get_child_element_1_response,
                path_children,
                type_children
            )
        );
        Assert.True(get_children.IsSuccess);
        GetMultipleChildrenResponse get_children_response =
            get_children.FromResult<GetMultipleChildrenResponse>();
        Assert.True(get_children_response.Results.Length > 0);

        ContractActionResult stop_driver_contract = await publisher.SendCommand(
            new StopWebDriverContract()
        );
        Assert.True(stop_driver_contract.IsSuccess);
        StopWebDriverContractResponse stop_driver_response =
            stop_driver_contract.FromResult<StopWebDriverContractResponse>();
        Assert.True(stop_driver_response.IsStopped);

        publisher.Dispose();
        await workerLife;
    }

    [Fact]
    public async Task Get_Text_From_Element()
    {
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task workerLife = worker.StartAsync(cancellationTokenSource.Token);

        MultiCommunicationPublisher publisher = new MultiCommunicationPublisher(
            queue,
            localhost,
            user,
            password
        );

        ContractActionResult start_contract = await publisher.SendCommand(
            new StartWebDriverContract("none")
        );
        Assert.True(start_contract.IsSuccess);
        StartWebDriverContractResponse start_response =
            start_contract.FromResult<StartWebDriverContractResponse>();
        Assert.True(start_response.IsStarted);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        OpenWebDriverPageResponse open_page_response =
            open_page_contract.FromResult<OpenWebDriverPageResponse>();
        Assert.Equal(avitoUrl, open_page_response.OpenedUrl);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        ScrollPageDownContractResponse scroll_page_down_response =
            scroll_page_down_contract.FromResult<ScrollPageDownContractResponse>();
        Assert.True(scroll_page_down_response.IsScrolled);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        ScrollPageTopResponse scroll_page_top_response =
            scroll_page_top_contract.FromResult<ScrollPageTopResponse>();
        Assert.True(scroll_page_top_response.IsScrolled);

        string path_element_1 = ".//div[@class='form-mainFilters-y0xZT']";
        string path_type_element_1 = "xpath";
        ContractActionResult get_element_1 = await publisher.SendCommand(
            new GetSingleElementContract(path_element_1, path_type_element_1)
        );
        Assert.True(get_element_1.IsSuccess);
        WebElementResponse get_element_1_response = get_element_1.FromResult<WebElementResponse>();
        Assert.Equal(path_element_1, get_element_1_response.ElementPath);
        Assert.Equal(path_type_element_1, get_element_1_response.ElementPathType);
        Assert.True(get_element_1_response.ElementId != Guid.Empty);

        string path_element_2 = "form";
        string type_element_2 = "tag";
        ContractActionResult get_child_element_1 = await publisher.SendCommand(
            new GetSingleChildElementContract(
                get_element_1_response,
                path_element_2,
                type_element_2
            )
        );
        Assert.True(get_child_element_1.IsSuccess);
        WebElementResponse get_child_element_1_response =
            get_child_element_1.FromResult<WebElementResponse>();
        Assert.Equal(path_element_2, get_child_element_1_response.ElementPath);
        Assert.Equal(type_element_2, get_child_element_1_response.ElementPathType);
        Assert.True(get_child_element_1_response.ElementId != Guid.Empty);

        string path_children =
            ".//div[@class='styles-module-root-G07MD styles-module-root_dense-kUp8z styles-module-root_compensate_bottom-WEqOQ']";
        string type_children = "xpath";
        ContractActionResult get_children = await publisher.SendCommand(
            new GetMultipleChildrenContract(
                get_child_element_1_response,
                path_children,
                type_children
            )
        );
        Assert.True(get_children.IsSuccess);
        GetMultipleChildrenResponse get_children_response =
            get_children.FromResult<GetMultipleChildrenResponse>();
        Assert.True(get_children_response.Results.Length > 0);

        ILogger logger = _serviceProvider.GetRequiredService<ILogger>();
        foreach (var item in get_children_response.Results)
        {
            ContractActionResult request = await publisher.SendCommand(
                new GetTextFromElementContract(item)
            );
            Assert.True(request.IsSuccess);
            GetTextFromElementResponse response = request.FromResult<GetTextFromElementResponse>();
            logger.Information("Text: {Text}", response.Text);
        }

        ContractActionResult stop_driver_contract = await publisher.SendCommand(
            new StopWebDriverContract()
        );
        Assert.True(stop_driver_contract.IsSuccess);
        StopWebDriverContractResponse stop_driver_response =
            stop_driver_contract.FromResult<StopWebDriverContractResponse>();
        Assert.True(stop_driver_response.IsStopped);

        publisher.Dispose();
        await workerLife;
    }

    [Fact]
    public async Task Test_Click_Element()
    {
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        Task workerLife = worker.StartAsync(cancellationTokenSource.Token);

        MultiCommunicationPublisher publisher = new MultiCommunicationPublisher(
            queue,
            localhost,
            user,
            password
        );

        ContractActionResult start_contract = await publisher.SendCommand(
            new StartWebDriverContract("none")
        );
        Assert.True(start_contract.IsSuccess);
        StartWebDriverContractResponse start_response =
            start_contract.FromResult<StartWebDriverContractResponse>();
        Assert.True(start_response.IsStarted);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        OpenWebDriverPageResponse open_page_response =
            open_page_contract.FromResult<OpenWebDriverPageResponse>();
        Assert.Equal(avitoUrl, open_page_response.OpenedUrl);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        ScrollPageDownContractResponse scroll_page_down_response =
            scroll_page_down_contract.FromResult<ScrollPageDownContractResponse>();
        Assert.True(scroll_page_down_response.IsScrolled);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        ScrollPageTopResponse scroll_page_top_response =
            scroll_page_top_contract.FromResult<ScrollPageTopResponse>();
        Assert.True(scroll_page_top_response.IsScrolled);

        string getElementPath = ".//button[@data-marker='top-rubricator/all-categories']";
        string getElementPathtype = "xpath";
        GetSingleElementContract getElement = new GetSingleElementContract(
            getElementPath,
            getElementPathtype
        );
        ContractActionResult getElementResult = await publisher.SendCommand(getElement);
        Assert.True(getElementResult.IsSuccess);
        WebElementResponse getElementResponse = getElementResult.FromResult<WebElementResponse>();
        Assert.Equal(getElementPath, getElementResponse.ElementPath);
        Assert.Equal(getElementPathtype, getElementResponse.ElementPathType);
        Assert.True(getElementResponse.ElementId != Guid.Empty);

        ClickOnElementContract clickOnElement = new ClickOnElementContract(getElementResponse);
        ContractActionResult clickResult = await publisher.SendCommand(clickOnElement);
        Assert.True(clickResult.IsSuccess);
        ClickOnElementResponse response = clickResult.FromResult<ClickOnElementResponse>();
        Assert.True(response.IsClicked);

        ContractActionResult stop_driver_contract = await publisher.SendCommand(
            new StopWebDriverContract()
        );
        Assert.True(stop_driver_contract.IsSuccess);
        StopWebDriverContractResponse stop_driver_response =
            stop_driver_contract.FromResult<StopWebDriverContractResponse>();
        Assert.True(stop_driver_response.IsStopped);

        publisher.Dispose();
        await workerLife;
    }
}
