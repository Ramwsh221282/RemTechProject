using System.Net;

namespace RemTech.WebApi.Envelope;

public sealed record EnvelopeResult(HttpStatusCode Code, string? StatusInfo, object? Data);
