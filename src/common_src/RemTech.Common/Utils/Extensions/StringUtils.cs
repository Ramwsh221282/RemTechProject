using System.Text.RegularExpressions;

namespace RemTechCommon.Utils.Extensions;

public static class StringUtils
{
    public static string CleanString(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "";

        input = input.Replace("\r", " ").Replace("\n", " ");
        input = Regex.Replace(input, @"[^\w\s]", " "); // Оставляем только буквы, цифры и пробелы
        input = Regex.Replace(input.Trim(), @"\s+", " "); // Заменяем множественные пробелы на один

        return input;
    }
}
