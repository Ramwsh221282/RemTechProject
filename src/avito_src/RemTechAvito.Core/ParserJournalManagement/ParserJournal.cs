using RemTechAvito.Core.Common.ValueObjects;

namespace RemTechAvito.Core.ParserJournalManagement;

public sealed class ParserJournal
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Time Time { get; private set; } = Time.Create(0, 0, 0);
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public int ItemsParsed { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public string Source { get; private set; } = string.Empty;

    public DateCreated CreatedOn { get; private set; } = DateCreated.Current();

    private ParserJournal() { }

    public static ParserJournal CreateSuccess(
        Time time,
        int itemsParsed,
        string? source = null,
        string? description = null
    )
    {
        return new ParserJournal()
        {
            Time = time,
            ItemsParsed = itemsParsed,
            IsSuccess = true,
            Source = string.IsNullOrWhiteSpace(source) ? string.Empty : source,
            Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description,
        };
    }

    public static ParserJournal CreateFailure(
        Time time,
        string message,
        int itemsParsed = 0,
        string? source = null,
        string? description = null
    )
    {
        return new ParserJournal()
        {
            Time = time,
            ErrorMessage = message,
            IsSuccess = false,
            ItemsParsed = itemsParsed,
            Source = string.IsNullOrWhiteSpace(source) ? string.Empty : source,
            Description = string.IsNullOrWhiteSpace(description) ? string.Empty : description,
        };
    }
}
