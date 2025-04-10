using System.Text.Json;

namespace RemTech.Infrastructure.PostgreSql.ParserContext.DaoModels;

public sealed class ParserProfileDao
{
    public required Guid Id { get; set; }
    public required Guid ParserId { get; set; }
    public required string Name { get; set; }
    public required string State { get; set; }
    public required long RepeatEverySeconds { get; set; }
    public required long NextRunUnixSeconds { get; set; }
    public required string Links { get; set; }
}

public static class ParserProfileDaoExtensions
{
    public static string[] GetDeserializedLinksArray(this ParserProfileDao dao)
    {
        if (string.IsNullOrWhiteSpace(dao.Links))
            return [];

        using JsonDocument document = JsonDocument.Parse(dao.Links);
        JsonElement linkArray = document.RootElement.GetProperty("Links");
        string[] links =
        [
            .. linkArray.EnumerateArray().Select(item => item.GetProperty("Link").GetString()!),
        ];
        return links.ToArray();
    }
}
