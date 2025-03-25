using RemTech.MainApi.CharacteristicsManagement.Responses;
using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MainApi.CharacteristicsManagement.Features.GetCharacteristics;

public sealed record GetCharacteristicsRequest : IRequest<Option<CharacteristicResponse[]>>;
