using System.Text.RegularExpressions;
using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;

internal sealed class AdvertisementTextSearchQuery
    : IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>
{
    public void AddFilter(
        FilterAdvertisementsDto dto,
        List<FilterDefinition<TransportAdvertisement>> filters
    )
    {
        TextSearchDto? textSearchTerm = dto.Text;
        if (textSearchTerm == null)
            return;

        if (string.IsNullOrWhiteSpace(textSearchTerm.Text))
            return;

        string processedText = CleanText(textSearchTerm.Text);
        if (string.IsNullOrWhiteSpace(processedText))
            return;

        filters.Add(
            Builders<TransportAdvertisement>.Filter.Text(
                processedText,
                new TextSearchOptions() { CaseSensitive = false, DiacriticSensitive = false }
            )
        );
    }

    private static string CleanText(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        input = input.Replace("\r", " ").Replace("\n", " ");
        input = Regex.Replace(input, @"[^\w\s]", " "); // Оставляем только буквы, цифры и пробелы
        input = Regex.Replace(input.Trim(), @"\s+", " "); // Заменяем множественные пробелы на один

        return input;
    }
}
