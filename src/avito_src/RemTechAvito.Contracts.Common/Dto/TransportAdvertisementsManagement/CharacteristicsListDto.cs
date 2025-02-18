namespace RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;

public sealed record CharacteristicsListDto(CharacteristicDto[] Characteristics);

public sealed record CharacteristicDto(string Name, string Value);
