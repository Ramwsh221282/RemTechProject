namespace RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;

public abstract record RemoveTransportTypeQuery;

public sealed record RemoveSystemTransportTypeQuery : RemoveTransportTypeQuery;

public sealed record RemoveUserTransportTypeQuery(string Name, string Link)
    : RemoveTransportTypeQuery;
