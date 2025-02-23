namespace RemTechCommon.Utils.Extensions;

public static class UrlLinkValidator
{
    public static bool IsStringUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        bool isUri =
            Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        return isUri;
    }
}
