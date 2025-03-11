using System.Text.RegularExpressions;
using AvitoParser.PDK.Models.ValueObjects;
using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK;

public static class IdExtractor
{
    private static Regex _regex = new Regex(@"(\d+)\?context=", RegexOptions.Compiled);

    public static Result<ScrapedAdvertisementId> ExtractId(string input)
    {
        Match match = _regex.Match(input);
        if (!match.Success)
            return new Error("Input string doesn't match regex");

        string stringId = match.Groups[1].Value;
        Result<ScrapedAdvertisementId> resultId = ScrapedAdvertisementId.Create(stringId);
        return resultId;
    }
}
