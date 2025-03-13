using AvitoParser.PDK.Models;
using CompleteDromAdvertisementsPlugin.Extensions;
using CompleteDromAdvertisementsPlugin.Models;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using Serilog;

namespace CompleteDromAdvertisementsPlugin.Contexts;

public sealed class AdvertisemenetParsingContext
{
    private readonly IBrowser _browser;
    private readonly ScrapedAdvertisement[] _advertisements;
    private readonly ILogger _logger;
    private readonly PluginExecutionContext _context;
    private const int _maxConcurrentTasks = 5;

    public AdvertisemenetParsingContext(
        IBrowser browser,
        IEnumerable<ScrapedAdvertisement> advertisements,
        ILogger logger,
        PluginExecutionContext context
    ) =>
        (_browser, _advertisements, _logger, _context) = (
            browser,
            advertisements.ToArray(),
            logger,
            context
        );

    public async Task<IEnumerable<ScrapedAdvertisement>> Process()
    {
        List<ScrapedAdvertisement> collected = [];
        List<Task<ScrapedAdvertisement?>> tasks = [];

        int currentAdvertisementIndex = 0;

        while (
            tasks.Count < _maxConcurrentTasks && currentAdvertisementIndex < _advertisements.Length
        )
        {
            tasks.Add(Scrape(_advertisements[currentAdvertisementIndex]));
            _logger.Information(
                "Added ad in tasks lists: {Url}",
                _advertisements[currentAdvertisementIndex].SourceUrl.SourceUrl
            );
            currentAdvertisementIndex++;
        }

        while (tasks.Count > 0)
        {
            Task<ScrapedAdvertisement?> finished = await Task.WhenAny(tasks);
            tasks.Remove(finished);

            try
            {
                ScrapedAdvertisement? result = await finished;

                if (result != null)
                {
                    _logger.Information("Finished ad task: {Url}", result.SourceUrl.SourceUrl);
                    collected.Add(result);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(
                    "{Context}. {Exception}.",
                    nameof(AdvertisemenetParsingContext),
                    ex.Message
                );
            }

            if (currentAdvertisementIndex < _advertisements.Length)
            {
                tasks.Add(Scrape(_advertisements[currentAdvertisementIndex]));
                currentAdvertisementIndex++;
            }
        }

        return collected;
    }

    private async Task<ScrapedAdvertisement?> Scrape(ScrapedAdvertisement advertisement)
    {
        try
        {
            await using AdvertisementPage page = await AdvertisementPageFactory.Create(
                _browser,
                advertisement,
                _context,
                _logger
            );

            ScrapedAdvertisement copy = advertisement;
            copy = await page.ScrapePrice(copy);
            copy = await page.ScrapeCharacteristics(copy);
            copy = await page.ScrapeDescription(copy);
            copy = await page.ScrapeAddress(copy);
            copy = await page.ScrapeDate(copy);
            copy = await page.ScrapePublisher(copy);
            return copy;
        }
        catch
        {
            return null;
        }
    }
}
