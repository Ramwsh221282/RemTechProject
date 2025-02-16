using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing.PhotoParsing;

internal sealed class PhotoClickingBehavior : IWebDriverBehavior
{
    private readonly CatalogueItem _item;
    private readonly ILogger _logger;
    private const string Type = "xpath";
    private const string ImageWrapperListPath =
        ".//ul[@data-marker='image-preview/preview-wrapper']";
    private const string RightButtonPath = ".//div[@data-marker='image-frame/right-button']";
    private const string Name = "images-wrapper";

    public PhotoClickingBehavior(CatalogueItem item, ILogger logger)
    {
        _item = item;
        _logger = logger;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        _logger.Information("{Action} attempt to parse photos...", nameof(PhotoClickingBehavior));
        WebElementPool pool = new WebElementPool();
        await GetImageWrapper(pool, publisher, ct);
        await GetImageScroller(pool, publisher, ct);

        Result<WebElement> scroller = pool[^1];
        if (scroller.IsFailure)
        {
            _logger.Warning("{Action} no scroller button found.", nameof(PhotoClickingBehavior));
            return Result.Success();
        }

        Result<HtmlDocument> photosDocument = CreatePhotosDocument(pool);
        if (photosDocument.IsFailure)
        {
            _logger.Warning("{Action} no images found.", nameof(PhotoClickingBehavior));
            return photosDocument.Error;
        }

        HtmlNodeCollection imageNodes = photosDocument.Value.DocumentNode.SelectNodes(".//li");
        PhotoClickingQueue queue = new PhotoClickingQueue { Scroller = scroller };
        foreach (var node in imageNodes)
        {
            if (node == null)
                continue;
            queue.Enqueue(node);
        }

        _item.PhotoLinks = new string[queue.PhotosCount];
        int lastNotInitializedImageIndex = 0;
        await foreach (var photo in ExtractPhotoLinks(queue, scroller, publisher, ct))
        {
            _item.PhotoLinks[lastNotInitializedImageIndex] = photo;
            lastNotInitializedImageIndex++;
        }

        ClearPoolBehavior clear = new ClearPoolBehavior();
        await clear.Execute(publisher, ct);
        return Result.Success();
    }

    private async Task GetImageWrapper(
        WebElementPool pool,
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        GetNewElementRetriable request = new GetNewElementRetriable(
            pool,
            ImageWrapperListPath,
            Type,
            Name,
            10
        );
        await request.Execute(publisher, ct);
    }

    private async Task GetImageScroller(
        WebElementPool pool,
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        GetNewElementRetriable request = new GetNewElementRetriable(
            pool,
            RightButtonPath,
            Type,
            "scroller",
            10
        );
        await request.Execute(publisher, ct);
    }

    private Result<HtmlDocument> CreatePhotosDocument(WebElementPool pool)
    {
        Result<WebElement> wrapper = pool[^2];
        if (wrapper.IsFailure)
            return new Error("Cannot find image wrapper.");

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(wrapper.Value.Model.ElementOuterHTML);
        return doc;
    }

    private async IAsyncEnumerable<string> ExtractPhotoLinks(
        PhotoClickingQueue queue,
        WebElement scroller,
        IMessagePublisher publisher,
        CancellationToken ct = default
    )
    {
        while (queue.QueueCount != 0)
        {
            var clicking = queue.Dequeue();
            if (clicking.ShouldSkip)
            {
                await queue.PerformClick(publisher, ct);
                _logger.Warning("{Action} not an image", nameof(PhotoClickingBehavior));
            }
            else
            {
                WebElementPool pool = new WebElementPool();
                var getImage = new GetNewElementRetriable(
                    pool,
                    ".//div[@class='image-frame-root-vKeXJ']",
                    Type,
                    "image",
                    10
                );

                await getImage.Execute(publisher, ct);
                Result<WebElement> image = pool[^1];
                if (image.IsFailure)
                {
                    yield return string.Empty;
                    continue;
                }

                HtmlDocument imageDocument = new HtmlDocument();
                imageDocument.LoadHtml(image.Value.Model.ElementOuterHTML);
                HtmlNode imageNode = imageDocument.DocumentNode.SelectSingleNode(".//img");
                if (imageNode == null)
                    continue;

                foreach (var attribute in imageNode.Attributes)
                {
                    if (attribute == null || attribute.Name != "src")
                        continue;
                    _logger.Information(
                        "{Action} got image {Url}",
                        nameof(PhotoClickingBehavior),
                        attribute.Value
                    );
                    await queue.PerformClick(publisher, ct);
                    yield return attribute.Value;
                }
            }
        }
    }
}
