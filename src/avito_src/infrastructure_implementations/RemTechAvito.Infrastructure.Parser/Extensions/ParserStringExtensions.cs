namespace RemTechAvito.Infrastructure.Parser.Extensions;

internal static class ParserStringExtensions
{
    public static string CleanString(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        input = input.Trim().RemoveNbSpIfExists().RemoveStars().RemoveAllNewLines();
        return input;
    }

    private static string RemoveNbSpIfExists(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        if (input.Contains("&nbsp;"))
            return input.Replace("&nbsp;", " ");

        return !input.Contains("&nbsp") ? input : input.Replace("&nbsp", " ");
    }

    private static string RemoveAllNewLines(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        if (!input.Contains("\r\n"))
            return input;

        ReadOnlySpan<char> span = input.AsSpan();
        int newLength = 0;

        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == '\r' && i + 1 < span.Length && span[i + 1] == '\n')
            {
                newLength++;
                i++;
            }
            else
            {
                newLength++;
            }
        }

        char[] resultArray = new char[newLength];
        int resultIndex = 0;

        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == '\r' && i + 1 < span.Length && span[i + 1] == '\n')
            {
                resultArray[resultIndex++] = ' ';
                i++;
            }
            else
            {
                resultArray[resultIndex++] = span[i];
            }
        }

        return new string(resultArray);
    }

    private static string RemoveStars(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        if (!input.Contains('*'))
            return input;

        ReadOnlySpan<char> initial = input.AsSpan();
        int indexOfFirstStart = initial.IndexOf('*');
        char[] withoutStars = new char[indexOfFirstStart];
        for (int index = 0; index < withoutStars.Length; index++)
            withoutStars[index] = initial[index];

        ReadOnlySpan<char> result = new ReadOnlySpan<char>(withoutStars);
        return result.ToString();
    }
}
