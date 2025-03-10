﻿using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.CatalogueParsing;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models;

internal sealed class CataloguePageModel
{
    private readonly IMessagePublisher _publisher;
    private readonly ILogger _logger;
    public string Url { get; }

    private readonly Queue<CatalogueItem> _items = new();
    private IEnumerable<long>? _existingIds;

    public CataloguePageModel(
        string url,
        IMessagePublisher publisher,
        ILogger logger,
        IEnumerable<long>? existingIds = null
    )
    {
        _existingIds = existingIds;
        Url = url;
        _publisher = publisher;
        _logger = logger;
        _logger.Information("Created {Class}. Url: {Url}", nameof(CataloguePageModel), url);
    }

    public async Task Initialize(CancellationToken ct = default)
    {
        var openPage = new OpenPageBehavior(Url);
        await openPage.Execute(_publisher, ct);

        var execution = new ParseCataloguePageBehavior(this, _logger);
        await execution.Execute(_publisher, ct);
    }

    public void AddItem(CatalogueItem item)
    {
        _items.Enqueue(item);
    }

    public IEnumerable<CatalogueItem> GetItems()
    {
        while (_items.Count != 0)
            yield return _items.Dequeue();
    }
}
