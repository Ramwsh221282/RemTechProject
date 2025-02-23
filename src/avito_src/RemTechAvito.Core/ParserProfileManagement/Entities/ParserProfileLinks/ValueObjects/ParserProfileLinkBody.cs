using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks.ValueObjects;

public sealed record ParserProfileLinkBody
{
    public string Mark { get; }
    public string Link { get; }

    private ParserProfileLinkBody(string mark, string link)
    {
        Mark = mark;
        Link = link;
    }

    public static Result<ParserProfileLinkBody> Create(string? mark, string? link)
    {
        if (string.IsNullOrWhiteSpace(mark))
            return new Error("Parser profile mark is required");

        if (string.IsNullOrWhiteSpace(link))
            return new Error("Parser profile link is required");

        bool isUri = UrlLinkValidator.IsStringUrl(link);

        return isUri
            ? new ParserProfileLinkBody(mark, link)
            : new Error("Provided link is not url");
    }
}
