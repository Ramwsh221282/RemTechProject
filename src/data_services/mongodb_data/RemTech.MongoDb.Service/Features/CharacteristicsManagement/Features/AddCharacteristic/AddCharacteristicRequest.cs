using RemTech.MongoDb.Service.Features.CharacteristicsManagement.Models;
using RemTechCommon.Utils.CqrsPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MongoDb.Service.Features.CharacteristicsManagement.Features.AddCharacteristic;

public sealed record AddCharacteristicRequest(Characteristic Characteristic) : IRequest<Result>;
