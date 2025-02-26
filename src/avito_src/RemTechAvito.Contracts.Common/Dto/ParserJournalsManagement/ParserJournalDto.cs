namespace RemTechAvito.Contracts.Common.Dto.ParserJournalsManagement;

public sealed record ParserJournalsResponse(IEnumerable<ParserJournalDto> Journals, int Count);

public sealed class ParserJournalDto
{
    public Guid Id { get; set; }
    public bool IsSuccess { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public int Hours { get; set; } = 0;
    public int Minutes { get; set; } = 0;
    public int Seconds { get; set; } = 0;
    public int ItemsParsed { get; set; } = 0;
    public string Error { get; set; } = string.Empty;
    public DateOnly CreatedOn { get; set; }
}
