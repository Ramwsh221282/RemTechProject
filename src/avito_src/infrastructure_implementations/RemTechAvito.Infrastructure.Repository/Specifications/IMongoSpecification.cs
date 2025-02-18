using MongoDB.Driver;
using RemTechAvito.Core.Common.Specifications;

namespace RemTechAvito.Infrastructure.Repository.Specifications;

public interface IMongoSpecification<T> : ISpecification<T>
{
    FilterDefinition<T> ToFilterDefinition();
}
