using RemTechCommon.Utils.ResultPattern;

namespace RemTechCommon.Utils.Converters;

public static class StringConverters
{
    public static Result<ulong> ToUlong(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Error("Text is empty");
        bool success = ulong.TryParse(text, out var result);
        if (!success)
            return new Error("Cannot be converted to ulong");
        return result;
    }

    public static Result<long> ToLong(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Error("Text is empty");
        bool success = long.TryParse(text, out var result);
        if (!success)
            return new Error("Cannot convert to long");
        return result;
    }

    public static Result<uint> ToUint(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Error("Text is empty");
        bool isConvertable = uint.TryParse(text, out uint result);
        if (!isConvertable)
            return new Error("Cannot convert to uint");
        return result;
    }

    public static Result<int> ToInt(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Error("Text is empty");
        bool isConvertable = int.TryParse(text, out int value);
        if (!isConvertable)
            return new Error("Cannot convert to int");
        return value;
    }
}
