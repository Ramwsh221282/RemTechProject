using System.Diagnostics;
using System.Net;
using RemTech.Shared.SDK.OptionPattern;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.WebApi.Envelope;

public static class EnvelopeResultExtensions
{
    public static IResult AsEnvelope<T>(this UnitResult<T> result)
    {
        HttpStatusCode code = result.MatchStatusCode();
        string info = result.Message;
        object? data = result.Result;
        EnvelopeResult envelope = new(code, info, data);
        return envelope.MatchEnvelope();
    }

    public static IResult AsEnvelope<T>(this Option<T> option) =>
        option.HasValue switch
        {
            true => UnitResult<T>.FromSuccess(option.Value).AsEnvelope(),
            false => UnitResult<T>.FromSuccess(default!).AsEnvelope(),
        };

    private static HttpStatusCode MatchStatusCode<T>(this UnitResult<T> result) =>
        result.Code switch
        {
            UnitResultCodes.Ok => HttpStatusCode.OK,
            UnitResultCodes.Created => HttpStatusCode.Created,
            UnitResultCodes.NoContent => HttpStatusCode.NoContent,
            UnitResultCodes.Unauthorized => HttpStatusCode.Unauthorized,
            UnitResultCodes.Forbidden => HttpStatusCode.Forbidden,
            UnitResultCodes.InternalError => HttpStatusCode.InternalServerError,
            UnitResultCodes.BadRequest => HttpStatusCode.BadRequest,
            UnitResultCodes.NotFound => HttpStatusCode.NotFound,
            _ => throw new UnreachableException("Неизвестный статус код Unit Result."),
        };

    private static IResult MatchEnvelope(this EnvelopeResult envelope) =>
        envelope.Code switch
        {
            HttpStatusCode.OK => Results.Ok(envelope),
            HttpStatusCode.Created => Results.Created(),
            HttpStatusCode.NoContent => Results.NoContent(),
            HttpStatusCode.Unauthorized => Results.Unauthorized(),
            HttpStatusCode.Forbidden => Results.Forbid(),
            HttpStatusCode.InternalServerError => Results.InternalServerError(),
            HttpStatusCode.BadRequest => Results.BadRequest(envelope.StatusInfo),
            HttpStatusCode.NotFound => Results.NotFound(),
            _ => throw new UnreachableException("Неизвестный статус код Unit Result."),
        };
}
