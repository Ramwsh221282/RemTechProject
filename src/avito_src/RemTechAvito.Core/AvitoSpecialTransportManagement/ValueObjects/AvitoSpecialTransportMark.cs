using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record AvitoSpecialTransportMark
{
    public string Mark { get; }

    private AvitoSpecialTransportMark(string mark) => Mark = mark;

    public static Result<AvitoSpecialTransportMark> Create(string? mark) =>
        string.IsNullOrWhiteSpace(mark)
            ? new Error("Special transport mark cannot be empty")
            : new AvitoSpecialTransportMark(mark);
}
