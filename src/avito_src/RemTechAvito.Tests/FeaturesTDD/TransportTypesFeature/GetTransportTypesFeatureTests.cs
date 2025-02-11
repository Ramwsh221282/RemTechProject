using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Tests.FeaturesTDD.TransportTypesFeature.Models;
using Serilog;
using WebDriver.Application.Queries.GetElementAttribute;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts;
using WebDriver.Worker.Service.Contracts.Responses;

namespace RemTechAvito.Tests.FeaturesTDD.TransportTypesFeature;

public sealed class GetTransportTypesFeatureTests
{
    private const string localhost = "localhost";
    private const string user = "rmuser";
    private const string password = "rmpassword";
    private const string queue = "web-driver-service";
    private const string avitoUrl =
        "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    public GetTransportTypesFeatureTests()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddLogging();
        collection.InitializeWorkerDependencies(queue, localhost, user, password);
        collection.AddSingleton<Worker>();
        _serviceProvider = collection.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger>();
    }

    [Fact]
    public async Task Get_Transport_Types_Checkboxes()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        MultiCommunicationPublisher publisher = new MultiCommunicationPublisher(
            queue,
            localhost,
            user,
            password
        );

        ContractActionResult startDriver = await publisher.SendCommand(
            new StartWebDriverContract("none"),
            ct
        );
        Assert.True(startDriver.IsSuccess);

        ContractActionResult openPage = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl),
            ct
        );
        Assert.True(openPage.IsSuccess);

        ContractActionResult scrollDown = await publisher.SendCommand(
            new ScrollPageDownContract(),
            ct
        );
        Assert.True(scrollDown.IsSuccess);

        ContractActionResult scrollTop = await publisher.SendCommand(
            new ScrollPageTopContract(),
            ct
        );
        Assert.True(scrollTop.IsSuccess);

        ContractActionResult getTransportTypesFilterContainer = await publisher.SendCommand(
            new GetSingleElementContract(
                AvitoTransportTypesWebElement.InputElementPath,
                AvitoTransportTypesWebElement.InputElementPathType
            ),
            ct
        );
        Assert.True(getTransportTypesFilterContainer.IsSuccess);
        WebElementResponse filterContainer =
            getTransportTypesFilterContainer.FromResult<WebElementResponse>();

        ContractActionResult clickOnFiltersElement = await publisher.SendCommand(
            new ClickOnElementContract(filterContainer),
            ct
        );
        Assert.True(clickOnFiltersElement.IsSuccess);

        ContractActionResult getFilterLists = await publisher.SendCommand(
            new GetSingleElementContract(
                AvitoTransportTypesWebElement.ElementsListContainerPath,
                AvitoTransportTypesWebElement.InputElementPathType
            ),
            ct
        );
        Assert.True(getFilterLists.IsSuccess);
        WebElementResponse filterList = getFilterLists.FromResult<WebElementResponse>();

        ContractActionResult getListItems = await publisher.SendCommand(
            new GetMultipleChildrenContract(
                filterList,
                AvitoTransportTypesWebElement.ElementCheckBoxPath,
                AvitoTransportTypesWebElement.InputElementPathType
            ),
            ct
        );
        Assert.True(getListItems.IsSuccess);
        WebElementResponse[] items = getListItems.FromResult<WebElementResponse[]>();

        foreach (var element in items)
        {
            ContractActionResult getText = await publisher.SendCommand(
                new GetTextFromElementContract(element),
                ct
            );
            Assert.True(getText.IsSuccess);
            string text = getText.FromResult<string>();
            _logger.Information("Text from checkbbox: {Text}", text);
            ContractActionResult click = await publisher.SendCommand(
                new ClickOnElementContract(element),
                ct
            );
        }

        ContractActionResult stop = await publisher.SendCommand(new StopWebDriverContract(), ct);
        Assert.True(stop.IsSuccess);

        publisher.Dispose();
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Click_On_Transport_Type_CheckBoxes_Test()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        MultiCommunicationPublisher publisher = new MultiCommunicationPublisher(
            queue,
            localhost,
            user,
            password
        );

        ContractActionResult startDriver = await publisher.SendCommand(
            new StartWebDriverContract("none"),
            ct
        );
        Assert.True(startDriver.IsSuccess);

        ContractActionResult openPage = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl),
            ct
        );
        Assert.True(openPage.IsSuccess);

        ContractActionResult scrollDown = await publisher.SendCommand(
            new ScrollPageDownContract(),
            ct
        );
        Assert.True(scrollDown.IsSuccess);

        ContractActionResult scrollTop = await publisher.SendCommand(
            new ScrollPageTopContract(),
            ct
        );
        Assert.True(scrollTop.IsSuccess);

        ContractActionResult getTransportTypesFilterContainer = await publisher.SendCommand(
            new GetSingleElementContract(
                AvitoTransportTypesWebElement.InputElementPath,
                AvitoTransportTypesWebElement.InputElementPathType
            ),
            ct
        );
        Assert.True(getTransportTypesFilterContainer.IsSuccess);
        WebElementResponse filterContainer =
            getTransportTypesFilterContainer.FromResult<WebElementResponse>();

        ContractActionResult clickOnFiltersElement = await publisher.SendCommand(
            new ClickOnElementContract(filterContainer),
            ct
        );
        Assert.True(clickOnFiltersElement.IsSuccess);

        ContractActionResult getFilterLists = await publisher.SendCommand(
            new GetSingleElementContract(
                AvitoTransportTypesWebElement.ElementsListContainerPath,
                AvitoTransportTypesWebElement.InputElementPathType
            ),
            ct
        );
        Assert.True(getFilterLists.IsSuccess);
        WebElementResponse filterList = getFilterLists.FromResult<WebElementResponse>();

        ContractActionResult getListItems = await publisher.SendCommand(
            new GetMultipleChildrenContract(
                filterList,
                AvitoTransportTypesWebElement.ElementCheckBoxPath,
                AvitoTransportTypesWebElement.InputElementPathType
            ),
            ct
        );
        Assert.True(getListItems.IsSuccess);
        WebElementResponse[] items = getListItems.FromResult<WebElementResponse[]>();

        int filterLimit = 0;
        foreach (var element in items)
        {
            if (filterLimit == 10)
                break;
            ContractActionResult getText = await publisher.SendCommand(
                new GetTextFromElementContract(element),
                ct
            );

            Assert.True(getText.IsSuccess);
            string text = getText.FromResult<string>();
            _logger.Information("Text from checkbbox: {Text}", text);

            while (true)
            {
                while (true) // clicking until is success click result.
                {
                    ContractActionResult click = await publisher.SendCommand(
                        new ClickOnElementContract(element),
                        ct
                    );
                    if (click.IsSuccess)
                        break;
                }

                ContractActionResult isCheckedAttribute = await publisher.SendCommand( // getting checkbox checked attribute
                    new GetElementAttributeValueContract(
                        element,
                        AvitoTransportTypesWebElement.CheckBoxCheckedAttribute
                    ),
                    ct
                );
                string result = isCheckedAttribute.FromResult<string>();
                if (!IsElementClicked(result)) // checking if checkbox is clicked
                    continue; // if not clicked repeat
                filterLimit++; // if clicked increment tracking limit variable and breaking
                break;
            }
        }

        ContractActionResult stop = await publisher.SendCommand(new StopWebDriverContract(), ct);
        Assert.True(stop.IsSuccess);

        publisher.Dispose();
        await worker.StopAsync(ct);
    }

    [Fact]
    public async Task Click_On_Transport_Type_CheckBoxes_With_Parameters_Test()
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

        MultiCommunicationPublisher publisher = new MultiCommunicationPublisher(
            queue,
            localhost,
            user,
            password
        );

        ContractActionResult startDriver = await publisher.SendCommand(
            new StartWebDriverContract("none"),
            ct
        );
        Assert.True(startDriver.IsSuccess);

        ContractActionResult openPage = await publisher.SendCommand(
            new OpenWebDriverPageContract(avitoUrl),
            ct
        );
        Assert.True(openPage.IsSuccess);

        ContractActionResult scrollDown = await publisher.SendCommand(
            new ScrollPageDownContract(),
            ct
        );
        Assert.True(scrollDown.IsSuccess);

        ContractActionResult scrollTop = await publisher.SendCommand(
            new ScrollPageTopContract(),
            ct
        );
        Assert.True(scrollTop.IsSuccess);

        ContractActionResult getTransportTypesFilterContainer = await publisher.SendCommand(
            new GetSingleElementContract(
                AvitoTransportTypesWebElement.InputElementPath,
                AvitoTransportTypesWebElement.InputElementPathType
            ),
            ct
        );
        Assert.True(getTransportTypesFilterContainer.IsSuccess);
        WebElementResponse filterContainer =
            getTransportTypesFilterContainer.FromResult<WebElementResponse>();

        ContractActionResult clickOnFiltersElement = await publisher.SendCommand(
            new ClickOnElementContract(filterContainer),
            ct
        );
        Assert.True(clickOnFiltersElement.IsSuccess);

        ContractActionResult getFilterLists = await publisher.SendCommand(
            new GetSingleElementContract(
                AvitoTransportTypesWebElement.ElementsListContainerPath,
                AvitoTransportTypesWebElement.InputElementPathType
            ),
            ct
        );
        Assert.True(getFilterLists.IsSuccess);
        WebElementResponse filterList = getFilterLists.FromResult<WebElementResponse>();

        ContractActionResult getListItems = await publisher.SendCommand(
            new GetMultipleChildrenContract(
                filterList,
                AvitoTransportTypesWebElement.ElementCheckBoxPath,
                AvitoTransportTypesWebElement.InputElementPathType
            ),
            ct
        );
        Assert.True(getListItems.IsSuccess);
        WebElementResponse[] items = getListItems.FromResult<WebElementResponse[]>();
        List<AvitoTransportTypesWebElement> types = [];
        foreach (var item in items)
        {
            ContractActionResult getText = await publisher.SendCommand(
                new GetTextFromElementContract(item),
                ct
            );
            string text = getText.FromResult<string>();
            if (parameters.Contains(text))
                types.Add(new AvitoTransportTypesWebElement() { Name = text, Element = item });
        }

        List<AvitoTransportTypesWebElement> clickedCheckboxNames = [];
        Assert.Equal(parameters.Length, types.Count);
        for (int index = 0; index < types.Count; index++)
        {
            while (true) // clicking until is success click result.
            {
                ContractActionResult click = await publisher.SendCommand(
                    new ClickOnElementContract(types[index].Element!),
                    ct
                );
                if (!click.IsSuccess)
                    continue;
                ContractActionResult isCheckedAttribute = await publisher.SendCommand( // getting checkbox checked attribute
                    new GetElementAttributeValueContract(
                        types[index].Element!,
                        AvitoTransportTypesWebElement.CheckBoxCheckedAttribute
                    ),
                    ct
                );
                string result = isCheckedAttribute.FromResult<string>();
                if (!IsElementClicked(result)) // checking if checkbox is clicked
                    continue; // if not clicked repeat
                clickedCheckboxNames.Add(types[index]);
                break;
            }
        }

        Assert.Equal(parameters.Length, clickedCheckboxNames.Count);

        ContractActionResult stop = await publisher.SendCommand(new StopWebDriverContract(), ct);
        Assert.True(stop.IsSuccess);

        publisher.Dispose();
        await worker.StopAsync(ct);
    }

    private bool IsElementClicked(ReadOnlySpan<char> received)
    {
        ReadOnlySpan<char> trueSequence = "true".AsSpan();
        return received.SequenceEqual(trueSequence);
    }
}
