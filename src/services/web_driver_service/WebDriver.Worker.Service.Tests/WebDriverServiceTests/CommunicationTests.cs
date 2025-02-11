using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using Serilog;
using WebDriver.Worker.Service.Contracts;
using WebDriver.Worker.Service.Contracts.Responses;

namespace WebDriver.Worker.Service.Tests.WebDriverServiceTests;

[Collection("Sequential")]
[CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
public sealed class CommunicationTests
{
    private const string localhost = "localhost";
    private const string user = "rmuser";
    private const string password = "rmpassword";
    private const string queue = "web-driver-service";
    private const string avitoUrl =
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    public CommunicationTests()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddLogging();
        collection.InitializeWorkerDependencies(queue, localhost, user, password);
        collection.AddSingleton<Worker>();
        _serviceProvider = collection.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger>();
    }

    [Fact]
    public async Task Test_Serialization_And_Deserialization()
    {
        _logger.Warning("Test 1");

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        await worker.StartAsync(cancellationTokenSource.Token);

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
        bool response_1 = result_1.FromResult<bool>();
        Assert.True(response_1);

        ContractActionResult result_2 = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(result_2.IsSuccess);
        bool response_2 = result_2.FromResult<bool>();
        Assert.True(response_2);

        ContractActionResult result_3 = await publisher.SendCommand(new StopWebDriverContract());
        Assert.True(result_3.IsSuccess);
        bool response = result_3.FromResult<bool>();
        Assert.True(response);

        await worker.StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task Test_GetSingle_Web_Element_Test()
    {
        _logger.Warning("Test 2");

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        await worker.StartAsync(cancellationTokenSource.Token);

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
        bool start_response = start_contract.FromResult<bool>();
        Assert.True(start_response);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        bool open_page_response = open_page_contract.FromResult<bool>();
        Assert.True(open_page_response);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        bool scroll_page_down_response = scroll_page_down_contract.FromResult<bool>();
        Assert.True(scroll_page_down_response);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        bool scroll_page_top_response = scroll_page_top_contract.FromResult<bool>();
        Assert.True(scroll_page_top_response);

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
        bool stop_driver_response = stop_driver_contract.FromResult<bool>();
        Assert.True(stop_driver_response);

        publisher.Dispose();
        await worker.StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task Test_Child_Element_Contract()
    {
        _logger.Warning("Test 3");

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        await worker.StartAsync(cancellationTokenSource.Token);

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
        bool start_response = start_contract.FromResult<bool>();
        Assert.True(start_response);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        bool open_page_response = open_page_contract.FromResult<bool>();
        Assert.True(open_page_response);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        bool scroll_page_down_response = scroll_page_down_contract.FromResult<bool>();
        Assert.True(scroll_page_down_response);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        bool scroll_page_top_response = scroll_page_top_contract.FromResult<bool>();
        Assert.True(scroll_page_top_response);

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
        bool stop_driver_response = stop_driver_contract.FromResult<bool>();
        Assert.True(stop_driver_response);

        publisher.Dispose();
        await worker.StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task Get_Multiple_Child_Elements_InParent_Test()
    {
        _logger.Warning("Test 4");

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        await worker.StartAsync(cancellationTokenSource.Token);

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
        bool start_response = start_contract.FromResult<bool>();
        Assert.True(start_response);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        bool open_page_response = open_page_contract.FromResult<bool>();
        Assert.True(open_page_response);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        bool scroll_page_down_response = scroll_page_down_contract.FromResult<bool>();
        Assert.True(scroll_page_down_response);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        bool scroll_page_top_response = scroll_page_top_contract.FromResult<bool>();
        Assert.True(scroll_page_top_response);

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
        WebElementResponse[] get_children_response =
            get_children.FromResult<WebElementResponse[]>();
        Assert.True(get_children_response.Length > 0);

        ContractActionResult stop_driver_contract = await publisher.SendCommand(
            new StopWebDriverContract()
        );
        Assert.True(stop_driver_contract.IsSuccess);
        bool stop_driver_response = stop_driver_contract.FromResult<bool>();
        Assert.True(stop_driver_response);

        publisher.Dispose();
        await worker.StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task Get_Text_From_Element()
    {
        _logger.Warning("Test 5");

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        await worker.StartAsync(cancellationTokenSource.Token);

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
        bool start_response = start_contract.FromResult<bool>();
        Assert.True(start_response);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        bool open_page_response = open_page_contract.FromResult<bool>();
        Assert.True(open_page_response);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        bool scroll_page_down_response = scroll_page_down_contract.FromResult<bool>();
        Assert.True(scroll_page_down_response);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        bool scroll_page_top_response = scroll_page_top_contract.FromResult<bool>();
        Assert.True(scroll_page_top_response);

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
        WebElementResponse[] get_children_response =
            get_children.FromResult<WebElementResponse[]>();
        Assert.True(get_children_response.Length > 0);

        foreach (var item in get_children_response)
        {
            ContractActionResult request = await publisher.SendCommand(
                new GetTextFromElementContract(item)
            );
            Assert.True(request.IsSuccess);
            request.FromResult<string>();
        }

        ContractActionResult stop_driver_contract = await publisher.SendCommand(
            new StopWebDriverContract()
        );
        Assert.True(stop_driver_contract.IsSuccess);
        bool stop_driver_response = stop_driver_contract.FromResult<bool>();
        Assert.True(stop_driver_response);

        publisher.Dispose();
        await worker.StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task Test_Click_Element()
    {
        _logger.Warning("Test 6");

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        await worker.StartAsync(cancellationTokenSource.Token);

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
        bool start_response = start_contract.FromResult<bool>();
        Assert.True(start_response);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        bool open_page_response = open_page_contract.FromResult<bool>();
        Assert.True(open_page_response);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        bool scroll_page_down_response = scroll_page_down_contract.FromResult<bool>();
        Assert.True(scroll_page_down_response);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        bool scroll_page_top_response = scroll_page_top_contract.FromResult<bool>();
        Assert.True(scroll_page_top_response);

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
        bool response = clickResult.FromResult<bool>();
        Assert.True(response);

        ContractActionResult stop_driver_contract = await publisher.SendCommand(
            new StopWebDriverContract()
        );
        Assert.True(stop_driver_contract.IsSuccess);
        bool stop_driver_response = stop_driver_contract.FromResult<bool>();
        Assert.True(stop_driver_response);

        publisher.Dispose();
        await worker.StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task Extract_HTML_Test()
    {
        _logger.Warning("Test 7");

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        await worker.StartAsync(cancellationTokenSource.Token);

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
        bool start_response = start_contract.FromResult<bool>();
        Assert.True(start_response);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        bool open_page_response = open_page_contract.FromResult<bool>();
        Assert.True(open_page_response);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        bool scroll_page_down_response = scroll_page_down_contract.FromResult<bool>();
        Assert.True(scroll_page_down_response);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        bool scroll_page_top_response = scroll_page_top_contract.FromResult<bool>();
        Assert.True(scroll_page_top_response);

        ContractActionResult extract_html = await publisher.SendCommand(new GetPageHtmlContract());
        Assert.True(extract_html.IsSuccess);
        string response = extract_html.FromResult<string>();
        Assert.NotEqual(string.Empty, response);
        _logger.Information(response);
        ContractActionResult stopping = await publisher.SendCommand(new StopWebDriverContract());
        Assert.True(stopping.IsSuccess);

        publisher.Dispose();
        await worker.StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task Extract_Element_Html_Query()
    {
        _logger.Warning("Test 7");

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        await worker.StartAsync(cancellationTokenSource.Token);

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
        bool start_response = start_contract.FromResult<bool>();
        Assert.True(start_response);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        bool open_page_response = open_page_contract.FromResult<bool>();
        Assert.True(open_page_response);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        bool scroll_page_down_response = scroll_page_down_contract.FromResult<bool>();
        Assert.True(scroll_page_down_response);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        bool scroll_page_top_response = scroll_page_top_contract.FromResult<bool>();
        Assert.True(scroll_page_top_response);

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

        ContractActionResult getElementHtml = await publisher.SendCommand(
            new GetElementHtmlContract(getElementResponse)
        );
        Assert.True(getElementHtml.IsSuccess);
        string html = getElementHtml.FromResult<string>();
        Assert.NotEqual(string.Empty, html);
        _logger.Information("{Html}", html);

        ContractActionResult stopping = await publisher.SendCommand(new StopWebDriverContract());
        Assert.True(stopping.IsSuccess);

        publisher.Dispose();
        await worker.StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task Send_Text_On_Element()
    {
        _logger.Warning("Test 8");

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        await worker.StartAsync(cancellationTokenSource.Token);

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
        bool start_response = start_contract.FromResult<bool>();
        Assert.True(start_response);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        bool open_page_response = open_page_contract.FromResult<bool>();
        Assert.True(open_page_response);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        bool scroll_page_down_response = scroll_page_down_contract.FromResult<bool>();
        Assert.True(scroll_page_down_response);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        bool scroll_page_top_response = scroll_page_top_contract.FromResult<bool>();
        Assert.True(scroll_page_top_response);

        string getElementPath = ".//input[@data-marker='search-form/suggest/input']";
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

        string text = "Люгонг";
        ContractActionResult sendText = await publisher.SendCommand(
            new SendTextOnElementContract(getElementResponse, text)
        );
        Assert.True(sendText.IsSuccess);
        bool isSendedText = sendText.FromResult<bool>();
        Assert.True(isSendedText);

        ContractActionResult stopping = await publisher.SendCommand(new StopWebDriverContract());
        Assert.True(stopping.IsSuccess);

        publisher.Dispose();
        await worker.StopAsync(cancellationTokenSource.Token);
    }

    [Fact]
    public async Task Get_Element_Attribute_Value()
    {
        _logger.Warning("Test 9");

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        await worker.StartAsync(cancellationTokenSource.Token);

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
        bool start_response = start_contract.FromResult<bool>();
        Assert.True(start_response);

        ContractActionResult open_page_contract = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl)
        );
        Assert.True(open_page_contract.IsSuccess);
        bool open_page_response = open_page_contract.FromResult<bool>();
        Assert.True(open_page_response);

        ContractActionResult scroll_page_down_contract = await publisher.SendCommand(
            new ScrollPageDownContract()
        );
        Assert.True(scroll_page_down_contract.IsSuccess);
        bool scroll_page_down_response = scroll_page_down_contract.FromResult<bool>();
        Assert.True(scroll_page_down_response);

        ContractActionResult scroll_page_top_contract = await publisher.SendCommand(
            new ScrollPageTopContract()
        );
        Assert.True(scroll_page_top_contract.IsSuccess);
        bool scroll_page_top_response = scroll_page_top_contract.FromResult<bool>();
        Assert.True(scroll_page_top_response);

        string getElementPath = ".//input[@data-marker='search-form/suggest/input']";
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

        string text = "Люгонг";
        ContractActionResult sendText = await publisher.SendCommand(
            new SendTextOnElementContract(getElementResponse, text)
        );
        Assert.True(sendText.IsSuccess);
        bool isSendedText = sendText.FromResult<bool>();
        Assert.True(isSendedText);

        string attribute = "value";
        ContractActionResult getAttribute = await publisher.SendCommand(
            new GetElementAttributeValueContract(getElementResponse, attribute)
        );
        string attributeValue = getAttribute.FromResult<string>();
        _logger.Information("Attribute value: {Value}", attributeValue);
        Assert.Equal(text, attributeValue);

        ContractActionResult stopping = await publisher.SendCommand(new StopWebDriverContract());
        Assert.True(stopping.IsSuccess);

        publisher.Dispose();
        await worker.StopAsync(cancellationTokenSource.Token);
    }
}
