using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Integrational.Tests.BaseCatalogueParsingTest.ConcretePropertiesTests;

public sealed class Parse_Photos_Tests : BasicParserTests
{
    public sealed record PhotoClicking(bool ShouldSkip);

    public sealed record PhotoClickingQueue
    {
        private readonly Queue<PhotoClicking> _queue = new Queue<PhotoClicking>();
        public int Count => _queue.Count;

        public void Add(PhotoClicking clicking) => _queue.Enqueue(clicking);

        public PhotoClicking Dequeue() => _queue.Dequeue();
    }

    [Fact]
    public async Task Parse_Photos_Sample_1()
    {
        const string url =
            "https://www.avito.ru/moskva/gruzzoviki_i_spetstehnika/vilochnyy_pogruzchik_hifoune_fd20t_2025_4404818555?context==H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiJLLNUZ0T215U054b1M5aTJGIjt9R6G3ST8AAAA";
        const string type = "xpath";
        const string imageWrapperListPath = ".//ul[@data-marker='image-preview/preview-wrapper']";
        const string rightButtonPath = ".//div[@data-marker='image-frame/right-button']";
        const string name = "images-wrapper";

        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        try
        {
            IMessagePublisher publisher = new MultiCommunicationPublisher(
                queue,
                host,
                user,
                password
            );
            WebElementPool pool = new WebElementPool();
            using WebDriverSession session = new WebDriverSession(publisher);
            await session.ExecuteBehavior(new StartBehavior("none"), ct);
            await session.ExecuteBehavior(new OpenPageBehavior(url), ct);
            await session.ExecuteBehavior(new ScrollToBottomRetriable(5), ct);
            await session.ExecuteBehavior(new ScrollToTopRetriable(5), ct);
            await session.ExecuteBehavior(
                new GetNewElementRetriable(pool, imageWrapperListPath, type, name, 5),
                ct
            );
            await session.ExecuteBehavior(
                new GetNewElementRetriable(pool, rightButtonPath, type, "scroller", 5),
                ct
            );

            Result<WebElement> wrapper = pool[^2];
            Assert.True(wrapper.IsSuccess);

            Result<WebElement> scroller = pool[^1];
            Assert.True(scroller.IsSuccess);

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(wrapper.Value.Model.ElementOuterHTML);
            HtmlNodeCollection? items = document.DocumentNode.SelectNodes(".//li");
            Assert.NotNull(items);
            Assert.Equal(9, items.Count);

            PhotoClickingQueue clickQueue = new PhotoClickingQueue();

            int imageArraySize = 0;
            int lastNotInitializedImageIndex = 0;

            foreach (var item in items)
            {
                if (item == null)
                    continue;

                IEnumerable<HtmlAttribute> attributes = item.Attributes;
                HtmlAttribute? typeAttribute = attributes.FirstOrDefault(a =>
                    a.Name == "data-type"
                );

                if (typeAttribute == null)
                    continue;

                if (typeAttribute.Value == "image")
                {
                    clickQueue.Add(new PhotoClicking(false));
                    imageArraySize++;
                    continue;
                }

                clickQueue.Add(new PhotoClicking(true));
            }

            string[] imageLinksArray = new string[imageArraySize];

            while (clickQueue.Count != 0)
            {
                bool shouldSkip = clickQueue.Dequeue().ShouldSkip;
                if (shouldSkip)
                    await session.ExecuteBehavior(
                        new ClickOnElementRetriable(scroller.Value, 10),
                        ct
                    );
                else
                {
                    await session.ExecuteBehavior(
                        new GetNewElementRetriable(
                            pool,
                            ".//div[@class='image-frame-root-vKeXJ']",
                            type,
                            "image",
                            10
                        ),
                        ct
                    );
                    Result<WebElement> image = pool[^1];
                    Assert.True(image.IsSuccess);
                    HtmlDocument imageDocument = new HtmlDocument();
                    imageDocument.LoadHtml(image.Value.Model.ElementOuterHTML);
                    HtmlNode imageNode = imageDocument.DocumentNode.SelectSingleNode(".//img");
                    if (imageNode == null)
                        continue;
                    IEnumerable<HtmlAttribute> imageAttributes = imageNode.Attributes;
                    HtmlAttribute? srcAttribute = imageAttributes.FirstOrDefault(a =>
                        a.Name == "src"
                    );
                    if (srcAttribute == null)
                        continue;
                    string imageLink = srcAttribute.Value;
                    imageLinksArray[lastNotInitializedImageIndex] = imageLink;
                    lastNotInitializedImageIndex++;
                    _logger.Information("image link: {image}", imageLink);
                    await session.ExecuteBehavior(
                        new ClickOnElementRetriable(scroller.Value, 10),
                        ct
                    );
                }
            }

            await session.ExecuteBehavior(new StopBehavior(), ct);

            foreach (var link in imageLinksArray)
                _logger.Information("{Link}", link);
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "Test running finished FATAL. Message: {Exception}. Source: {Source}",
                ex.Message,
                ex.Source
            );
        }
        finally
        {
            await worker.StopAsync(ct);
        }
    }
}
