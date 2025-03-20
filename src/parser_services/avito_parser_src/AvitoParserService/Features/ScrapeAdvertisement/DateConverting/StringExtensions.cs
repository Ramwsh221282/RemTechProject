using System.Text;

namespace AvitoParserService.Features.ScrapeAdvertisement.DateConverting;

public static class StringExtensions
{
    public static int GetIntegerFromString(this string input)
    {
        StringBuilder integers = new StringBuilder();
        for (int index = 0; index < input.Length; index++)
        {
            if (char.IsDigit(input[index]))
                integers.Append(input[index]);
        }

        return int.Parse(integers.ToString());
    }
}
