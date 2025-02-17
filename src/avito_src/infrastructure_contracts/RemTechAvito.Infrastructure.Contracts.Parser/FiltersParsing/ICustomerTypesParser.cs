using RemTechAvito.Core.FiltersManagement.CustomerTypes;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Parser.FiltersParsing;

public interface ICustomerTypesParser
{
    IAsyncEnumerable<Result<CustomerType>> Parse(CancellationToken ct = default);
}
