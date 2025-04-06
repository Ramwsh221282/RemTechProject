namespace RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.DaoModels;

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
