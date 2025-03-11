using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CreateAdvertisementPhotoList;

[Plugin(PluginName = nameof(CreateAdvertisementPhotoList))]
public sealed class CreateAdvertisementPhotoList : IPlugin<IReadOnlyCollection<ScrapedPhotoUrl>>
{
    const string currentImageContainerSelector = "image-frame-wrapper-_NvbY";

    const string imageListContainerSelector =
        "images-preview-previewWrapper-R_a4U images-preview-previewWrapper_newStyle-fGdrG";

    public async Task<Result<IReadOnlyCollection<ScrapedPhotoUrl>>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CreateAdvertisementPhotoList),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreateAdvertisementPhotoList),
                nameof(IPage)
            );
        IPage page = pageUnwrap.Value;

        IElementHandle? currentImageContainer = await page.QuerySelectorAsync(
            new QuerySelectorPayload(currentImageContainerSelector).Selector
        );

        if (currentImageContainer == null)
        {
            logger.Warning("{Context} no images found", nameof(CreateAdvertisementPhotoList));
            return Result<IReadOnlyCollection<ScrapedPhotoUrl>>.Success([]);
        }

        IElementHandle imageListContainer = await page.QuerySelectorAsync(
            new QuerySelectorPayload(imageListContainerSelector).Selector
        );
        if (imageListContainer == null)
        {
            logger.Warning("{Context} no images found", nameof(CreateAdvertisementPhotoList));
            return Result<IReadOnlyCollection<ScrapedPhotoUrl>>.Success([]);
        }

        IElementHandle? currentImage = await currentImageContainer.QuerySelectorAsync("img");
        if (currentImage == null)
        {
            logger.Warning("{Context} no images found", nameof(CreateAdvertisementPhotoList));
            return Result<IReadOnlyCollection<ScrapedPhotoUrl>>.Success([]);
        }

        IElementHandle[] images = await imageListContainer.QuerySelectorAllAsync("li");
        List<ScrapedPhotoUrl> results = [];
        Result<ScrapedPhotoUrl> currentImagePhoto = ScrapedPhotoUrl.Create(
            await currentImage.EvaluateFunctionAsync<string>($"el => el.getAttribute('src')")
        );
        if (currentImagePhoto.IsSuccess)
            results.Add(currentImagePhoto.Value);

        for (int index = 1; index < images.Length; index++)
        {
            string dataType = await images[index]
                .EvaluateFunctionAsync<string>("el => el.getAttribute('data-type')");
            if (dataType != "image")
                continue;
            await images[index].ClickAsync();
            string imgSrc = await currentImage.EvaluateFunctionAsync<string>(
                $"el => el.getAttribute('src')"
            );
            Result<ScrapedPhotoUrl> imgResult = ScrapedPhotoUrl.Create(imgSrc);
            if (imgResult.IsSuccess)
                results.Add(imgResult.Value);
        }

        return results;
    }
}
