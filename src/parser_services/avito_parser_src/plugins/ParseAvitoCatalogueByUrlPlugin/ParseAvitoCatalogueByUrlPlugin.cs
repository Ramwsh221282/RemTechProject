using AvitoParser.PDK;
using AvitoParser.PDK.Dtos;
using AvitoParser.PDK.Models;
using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace ParseAvitoCatalogueByUrlPlugin;

[Plugin(PluginName = nameof(ParseAvitoCatalogueByUrlPlugin))]
public sealed class ParseAvitoCatalogueByUrlPlugin : IPlugin<IEnumerable<ScrapedAdvertisement>>
{
    public async Task<Result<IEnumerable<ScrapedAdvertisement>>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(ParseAvitoCatalogueByUrlPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<PluginExecutionContext> contextUnwrap = resolver.Resolve<PluginExecutionContext>();
        if (contextUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(ParseAvitoCatalogueByUrlPlugin),
                nameof(PluginExecutionContext)
            );
        PluginExecutionContext context = contextUnwrap.Value;

        Result<ScrapedSourceUrl> catalogueUrlUnwrap = resolver.Resolve<ScrapedSourceUrl>();
        if (catalogueUrlUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(ParseAvitoCatalogueByUrlPlugin),
                nameof(ScrapedSourceUrl)
            );
        ScrapedSourceUrl catalogueUrl = catalogueUrlUnwrap.Value;

        Result<LoginDto> loginDataUnwrap = resolver.Resolve<LoginDto>();
        if (loginDataUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(ParseAvitoCatalogueByUrlPlugin),
                nameof(LoginDto)
            );
        LoginDto loginData = loginDataUnwrap.Value;

        Result<IBrowser> browserResult = await context.ExecutePlugin<IBrowser>(
            new PluginCommand(
                "CreateAvitoScraperBrowserInstancePlugin",
                new PluginPayload(logger, context)
            )
        );
        if (browserResult.IsFailure)
        {
            logger.Error(
                "{Context} can't instantiate browser",
                nameof(ParseAvitoCatalogueByUrlPlugin)
            );
            return new Error("Can't instantiate browser");
        }

        IBrowser browser = browserResult.Value;
        Result<IPage> authPageResult = await context.ExecutePlugin<IPage>(
            new PluginCommand(
                "LoginInAvitoPlugin",
                new PluginPayload(
                    logger,
                    browser,
                    new LoginDto(loginData.Email, loginData.Password)
                )
            )
        );
        if (authPageResult.IsFailure)
        {
            logger.Error("{Context} unable to login", nameof(ParseAvitoCatalogueByUrlPlugin));
            return new Error("Unable to login avito");
        }

        IPage authPage = authPageResult.Value;
        authPage.Dispose();

        Result<IPage> cataloguePageResult = await context.ExecutePlugin<IPage>(
            new PluginCommand(
                "CreateAvitoCataloguePagePlugin",
                new PluginPayload(logger, context, browser, catalogueUrl)
            )
        );
        if (cataloguePageResult.IsFailure)
        {
            logger.Error(
                "{Context} unable to create catalogue page",
                nameof(ParseAvitoCatalogueByUrlPlugin)
            );
            return new Error("Unable to create catalogue page");
        }

        IPage cataloguePage = cataloguePageResult.Value;
        Result<IEnumerable<ScrapedSourceUrl>> pagedUrls = await context.Execute<
            IEnumerable<ScrapedSourceUrl>
        >("CreatePagedCatalogueUrls", new PluginPayload(logger, cataloguePage, catalogueUrl));
        if (pagedUrls.IsFailure)
        {
            cataloguePage.Dispose();
            logger.Error(
                "{Context} unable to create paged urls",
                nameof(ParseAvitoCatalogueByUrlPlugin)
            );
            return new Error("Unable to create paged urls");
        }

        cataloguePage.Dispose();

        List<ScrapedAdvertisement> advertisements = [];
        IEnumerable<ScrapedSourceUrl> paginationUrls = pagedUrls.Value;
        foreach (ScrapedSourceUrl paginationUrl in paginationUrls)
        {
            Result<IPage> nextPage = await context.Execute<IPage>(
                "CreateAvitoCataloguePagePlugin",
                new PluginPayload(logger, context, browser, paginationUrl)
            );
            if (nextPage.IsFailure)
                continue;

            Result<IEnumerable<ScrapedSourceUrl>> advertisementLinksResult =
                await context.ExecutePlugin<IEnumerable<ScrapedSourceUrl>>(
                    new PluginCommand(
                        "CreateAvitoAdvertisementLinksListPlugin",
                        new PluginPayload(logger, context, nextPage.Value)
                    )
                );
            if (advertisementLinksResult.IsFailure)
            {
                nextPage.Value.Dispose();
                continue;
            }

            nextPage.Value.Dispose();
            ScrapedSourceUrl[] urls = advertisementLinksResult.Value.ToArray();
            foreach (var scrapedUrl in urls)
            {
                await Task.Run(async () =>
                {
                    Result<ScrapedAdvertisementId> id = IdExtractor.ExtractId(scrapedUrl.SourceUrl);

                    Result<IPage> advertisementPageResult = await context.ExecutePlugin<IPage>(
                        new PluginCommand(
                            "OpenAdvertisementPagePlugin",
                            new PluginPayload(logger, browser, scrapedUrl, context)
                        )
                    );

                    IPage advertisementPage = advertisementPageResult.Value;

                    Result<ScrapedTitle> title = await context.ExecutePlugin<ScrapedTitle>(
                        new PluginCommand(
                            "CreateAdvertisementTitlePlugin",
                            new PluginPayload(logger, advertisementPage)
                        )
                    );
                    if (title.IsFailure)
                    {
                        logger.Error(
                            "{Context}. {Error}",
                            nameof(ParseAvitoCatalogueByUrlPlugin),
                            title.Error.Description
                        );
                        advertisementPage.Dispose();
                        return;
                    }

                    Result<ScrapedPrice> priceResult = await context.Execute<ScrapedPrice>(
                        "CreateAvitoAdvertisementPricePlugin",
                        new PluginPayload(logger, advertisementPage, context)
                    );
                    if (priceResult.IsFailure)
                    {
                        logger.Error(
                            "{Context}. {Error}",
                            nameof(ParseAvitoCatalogueByUrlPlugin),
                            priceResult.Error.Description
                        );
                        advertisementPage.Dispose();
                        return;
                    }

                    Result<IReadOnlyCollection<ScrapedCharacteristic>> characteristics =
                        await context.Execute<IReadOnlyCollection<ScrapedCharacteristic>>(
                            "CreateAvitoAdvertisementCharacteristics",
                            new PluginPayload(logger, advertisementPage)
                        );
                    if (characteristics.IsFailure)
                    {
                        logger.Error(
                            "{Context}. {Error}",
                            nameof(ParseAvitoCatalogueByUrlPlugin),
                            characteristics.Error.Description
                        );
                        advertisementPage.Dispose();
                        return;
                    }

                    Result<ScrapedAddress> addressResult = await context.Execute<ScrapedAddress>(
                        "CreateAdvertisementAddressPlugin",
                        new PluginPayload(advertisementPage, logger)
                    );
                    if (addressResult.IsFailure)
                    {
                        logger.Error(
                            "{Context}. {Error}",
                            nameof(ParseAvitoCatalogueByUrlPlugin),
                            addressResult.Error.Description
                        );
                        advertisementPage.Dispose();
                        return;
                    }

                    Result<ScrapedDescription> descriptionResult =
                        await context.Execute<ScrapedDescription>(
                            "CreateAdvertisementDescriptionPlugin",
                            new PluginPayload(logger, advertisementPage)
                        );
                    if (descriptionResult.IsFailure)
                    {
                        logger.Error(
                            "{Context}. {Error}",
                            nameof(ParseAvitoCatalogueByUrlPlugin),
                            descriptionResult.Error.Description
                        );
                        advertisementPage.Dispose();
                        return;
                    }

                    Result<IReadOnlyCollection<ScrapedPhotoUrl>> photosResult =
                        await context.Execute<IReadOnlyCollection<ScrapedPhotoUrl>>(
                            "CreateAdvertisementPhotoList",
                            new PluginPayload(logger, advertisementPage)
                        );
                    if (photosResult.IsFailure)
                    {
                        logger.Error(
                            "{Context}. {Error}",
                            nameof(ParseAvitoCatalogueByUrlPlugin),
                            photosResult.Error.Description
                        );
                        advertisementPage.Dispose();
                        return;
                    }

                    Result<ScrapedAdvertisementDate> dateResult =
                        await context.Execute<ScrapedAdvertisementDate>(
                            "CreateAdvertisementDatePlugin",
                            new PluginPayload(logger, advertisementPage)
                        );
                    if (dateResult.IsFailure)
                    {
                        logger.Error(
                            "{Context}. {Error}",
                            nameof(ParseAvitoCatalogueByUrlPlugin),
                            dateResult.Error.Description
                        );
                        advertisementPage.Dispose();
                        return;
                    }

                    Result<ScrapedPublisher> publisherResult =
                        await context.Execute<ScrapedPublisher>(
                            "CreatePublisherPlugin",
                            new PluginPayload(logger, advertisementPage)
                        );
                    if (publisherResult.IsFailure)
                    {
                        logger.Error(
                            "{Context}. {Error}",
                            nameof(ParseAvitoCatalogueByUrlPlugin),
                            publisherResult.Error.Description
                        );
                        advertisementPage.Dispose();
                        return;
                    }

                    advertisementPage.Dispose();

                    ScrapedAdvertisement advertisement = new ScrapedAdvertisement();
                    advertisement = advertisement with
                    {
                        Id = id,
                        Address = addressResult,
                        Characteristics = characteristics.Value.ToList(),
                        Date = dateResult,
                        Description = descriptionResult,
                        Photos = photosResult.Value.ToList(),
                        Price = priceResult,
                        Publisher = publisherResult,
                        SourceUrl = scrapedUrl,
                        Title = title,
                    };

                    advertisements.Add(advertisement);

                    logger.Information(
                        "{Context} Advertisements count: {Count}",
                        nameof(ParseAvitoCatalogueByUrlPlugin),
                        advertisements.Count
                    );
                });
            }
        }

        await browser.DisposeAsync();
        return advertisements;
    }
}
