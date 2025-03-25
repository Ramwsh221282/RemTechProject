namespace RemTechCommon.Utils.CqrsPattern;

public interface IRequestHandler<TRequest, TRequestResult>
    where TRequest : IRequest<TRequestResult>
{
    Task<TRequestResult> Handle(TRequest request, CancellationToken ct = default);
}
