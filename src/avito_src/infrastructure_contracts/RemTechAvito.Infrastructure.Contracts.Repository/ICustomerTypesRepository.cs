using RemTechAvito.Core.FiltersManagement.CustomerTypes;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface ICustomerTypesRepository
{
    Task<Result> Add(CustomerTypesCollection collection, CancellationToken ct = default);
}
