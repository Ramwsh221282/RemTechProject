﻿using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;

public interface ITransportTypesParser
{
    IAsyncEnumerable<Result<SystemTransportType>> Parse(CancellationToken ct = default);
}
