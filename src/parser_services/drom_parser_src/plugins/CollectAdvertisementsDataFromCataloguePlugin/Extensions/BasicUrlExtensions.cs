using System.Text;
using AvitoParser.PDK.Models.ValueObjects;

namespace CollectAdvertisementsDataFromCataloguePlugin.Extensions;

public static class BasicUrlExtensions
{
    public static ScrapedSourceUrl CreatePageUrl(this ScrapedSourceUrl url, int page)
    {
        string basicUrl = url.SourceUrl;
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(basicUrl);
        stringBuilder.Append($"/page{page}/");
        string nextPageUrl = stringBuilder.ToString();
        return ScrapedSourceUrl.Create(nextPageUrl);
    }
}
