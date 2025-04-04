using System.Net;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.Common.Envelope;

public sealed record EnvelopeResult
{
    public HttpStatusCode Code { get; init; }
    public string StatusInfo { get; init; } = string.Empty;
    public object? Data { get; init; } = null;
}

public static class EnvelopeResultExtensions
{
    public static IResult Envelope<T>(T result, HttpStatusCode code = HttpStatusCode.OK)
    {
        EnvelopeResult envelope = new()
        {
            Code = code,
            StatusInfo = string.Empty,
            Data = result,
        };
        return envelope.MatchEnvelope();
    }
    
    public static IResult Envelope(HttpStatusCode code, string? message = null)
    {
        EnvelopeResult envelope = new()
        {
            Code = code,
            StatusInfo = message ?? string.Empty,
            Data = null,
        };
        return envelope.MatchEnvelope();
    }

    public static IResult NotFound()
    {
        EnvelopeResult envelope = new()
        {
            Code = HttpStatusCode.NotFound,
            StatusInfo = "Not found",
            Data = null,
        };
        return envelope.MatchEnvelope();
    }

    public static IResult Envelope<T>(
        this Result<T> result,
        string? statusInfo = null,
        HttpStatusCode code = HttpStatusCode.OK
    )
    {
        if (result.IsFailure)
            return result.Error.Envelope();

        EnvelopeResult envelope = new()
        {
            Code = code,
            StatusInfo = statusInfo ?? string.Empty,
            Data = result.Value,
        };

        return envelope.MatchEnvelope();
    }

    public static IResult Envelope(
        string errorMessage,
        HttpStatusCode code = HttpStatusCode.BadRequest
    )
    {
        EnvelopeResult envelope = new() { Code = code, StatusInfo = errorMessage };
        return envelope.MatchEnvelope();
    }

    public static IResult Envelope(this Error error) => Envelope(error.Description);

    private static IResult MatchEnvelope(this EnvelopeResult envelope) =>
        envelope.Code switch
        {
            HttpStatusCode.OK => Results.Ok(envelope),
            HttpStatusCode.Created => Results.Created(),
            HttpStatusCode.NoContent => Results.NoContent(),
            HttpStatusCode.BadRequest => Results.BadRequest(envelope.StatusInfo),
            HttpStatusCode.NotFound => Results.NotFound(),
            HttpStatusCode.InternalServerError => Results.InternalServerError(),
            _ => Results.InternalServerError("Envelope status is not supported."),
        };
}
