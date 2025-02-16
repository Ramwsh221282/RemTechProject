using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing.PhotoParsing;

public sealed class PhotoClickingQueue
{
    private readonly Queue<PhotoClicking> _queue = new Queue<PhotoClicking>();
    public int PhotosCount { get; private set; }
    public int QueueCount => _queue.Count;

    public WebElement? Scroller { get; set; }

    public void Enqueue(HtmlNode node)
    {
        PhotoClicking item = new PhotoClicking(node);
        if (!item.ShouldSkip)
            PhotosCount++;
        _queue.Enqueue(item);
    }

    public async Task PerformClick(IMessagePublisher publisher, CancellationToken ct = default)
    {
        if (Scroller == null)
            return;
        ClickOnElementRetriable click = new ClickOnElementRetriable(Scroller, 10);
        await click.Execute(publisher, ct);
    }

    public PhotoClicking Dequeue() => _queue.Dequeue();

    public sealed class PhotoClicking
    {
        public bool ShouldSkip { get; }

        public PhotoClicking(HtmlNode node)
        {
            if (
                node.Attributes.Any(attribute =>
                    attribute.Name == "data-type" && attribute.Value == "image"
                )
            )
            {
                ShouldSkip = false;
                return;
            }

            ShouldSkip = true;
        }
    }
}
