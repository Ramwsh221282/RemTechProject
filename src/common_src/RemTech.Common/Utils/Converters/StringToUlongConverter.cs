using RemTechCommon.Utils.ResultPattern;

namespace RemTechCommon.Utils.Converters;

public static class StringToUlongConverter
{
    public static Result<ulong> Convert(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Error("Id is empty");
        bool success = ulong.TryParse(text, out var result);
        if (!success)
            return new Error("Id cannot be converted to ulong");
        return result;
    }
}
