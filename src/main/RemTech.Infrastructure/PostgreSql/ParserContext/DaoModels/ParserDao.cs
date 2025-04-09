namespace RemTech.Infrastructure.PostgreSql.ParserContext.DaoModels;

public sealed class ParserDao
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required List<ParserProfileDao> Profiles { get; set; }
}
