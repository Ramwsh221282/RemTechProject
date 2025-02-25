namespace RemTechAvito.Contracts.Common.Responses.TransportTypesManagement;

public sealed record TransportTypeResponse(IEnumerable<TransportTypeDto> Items, long Count);

public sealed record TransportTypeDto(string Name, string Link);