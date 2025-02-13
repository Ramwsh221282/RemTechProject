using RemTechAvito.Core.FiltersManagement.CustomerStates;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface ICustomerStatesRepository
{
    Task<Result> Add(CustomerStatesCollection collection, CancellationToken ct = default);
}
