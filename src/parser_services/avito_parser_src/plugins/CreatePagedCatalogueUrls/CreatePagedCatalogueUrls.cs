using System.Text;
using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using Exception = System.Exception;

namespace CreatePagedCatalogueUrls;

[Plugin(PluginName = nameof(CreatePagedCatalogueUrls))]
public sealed class CreatePagedCatalogueUrls : IPlugin<IEnumerable<ScrapedSourceUrl>>
{
    private const string paginationContainerClass = "js-pages pagination-pagination-Oz4Ri";
    private const string script = "el => el.textContent";

    public async Task<Result<IEnumerable<ScrapedSourceUrl>>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CreatePagedCatalogueUrls),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreatePagedCatalogueUrls),
                nameof(IPage)
            );
        IPage page = pageUnwrap.Value;

        Result<ScrapedSourceUrl> basicUrl = resolver.Resolve<ScrapedSourceUrl>();
        if (basicUrl.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreatePagedCatalogueUrls),
                nameof(ScrapedSourceUrl)
            );
        ScrapedSourceUrl url = basicUrl.Value;

        try
        {
            QuerySelectorPayload paginationContainerSelector = new(paginationContainerClass);
            IElementHandle? elementHandle = await page.QuerySelectorAsync(
                paginationContainerSelector.Selector
            );
            if (elementHandle == null)
            {
                logger.Warning(
                    "{Context} catalogue has only one page",
                    nameof(CreatePagedCatalogueUrls)
                );
                return Result<IEnumerable<ScrapedSourceUrl>>.Success([url]);
            }

            IElementHandle[] paginationButtons = await elementHandle.QuerySelectorAllAsync("li");
            int maxPageNumber = 1;
            foreach (var button in paginationButtons)
            {
                string? textContent = await button.EvaluateFunctionAsync<string>(script);
                if (string.IsNullOrWhiteSpace(textContent))
                    continue;

                bool canParseInt = int.TryParse(textContent.Trim(), out int number);
                if (!canParseInt)
                    continue;

                if (number > maxPageNumber)
                    maxPageNumber = number;
            }

            List<ScrapedSourceUrl> results = CreateResults(basicUrl.Value.SourceUrl, maxPageNumber);
            return Result<IEnumerable<ScrapedSourceUrl>>.Success(results);
        }
        catch (Exception ex)
        {
            logger.Error(
                "{Context} internal plugin error occured. Exception: {Ex}",
                nameof(CreatePagedCatalogueUrls),
                ex.Message
            );
            return Result<IEnumerable<ScrapedSourceUrl>>.Success([url]);
        }
    }

    private static List<ScrapedSourceUrl> CreateResults(string basicPageUrl, int maxPageNumber)
    {
        List<ScrapedSourceUrl> results = [];
        for (int i = 1; i <= maxPageNumber; i++)
        {
            string page = CreateNextPageString(basicPageUrl, i);
            results.Add(ScrapedSourceUrl.Create(page));
        }

        return results;
    }

    private static string CreateNextPageString(string input, int page)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(input);
        stringBuilder.Append($"?p={page}");
        return stringBuilder.ToString();
    }
}
