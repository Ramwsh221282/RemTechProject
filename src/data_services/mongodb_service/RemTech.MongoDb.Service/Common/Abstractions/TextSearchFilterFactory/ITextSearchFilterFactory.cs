using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Dtos;

namespace RemTech.MongoDb.Service.Common.Abstractions.TextSearchFilterFactory;

public interface ITextSearchFilterFactory<TFilterEntity> where TFilterEntity : class
{
    public FilterDefinition<TFilterEntity> Apply(FilterDefinition<TFilterEntity> filter, TextSearchOption option);
}