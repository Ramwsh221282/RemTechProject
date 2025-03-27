using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Dtos;
using RemTech.MongoDb.Service.Configurations.MongoDbConfiguration;
using RemTech.MongoDb.Service.Extensions;
using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;

public sealed class AdvertisementsRepository
{
    private readonly MongoClient _client;

    public AdvertisementsRepository(MongoClient client) => _client = client;

    public async Task Save(Advertisement advertisement)
    {
        IMongoDatabase db = _client.GetAdvertisementsDb();
        IMongoCollection<Advertisement> col = db.GetAdvertisementsCol();
        await col.InsertOneAsync(advertisement);
    }

    public async Task<bool> Contains(FilterDefinition<Advertisement> filter)
    {
        IMongoDatabase db = _client.GetAdvertisementsDb();
        IMongoCollection<Advertisement> col = db.GetAdvertisementsCol();
        IFindFluent<Advertisement, Advertisement> find = col.Find(filter);
        return await find.AnyAsync();
    }

    public async Task<Option<Advertisement>> Get(FilterDefinition<Advertisement> filter)
    {
        IMongoDatabase db = _client.GetAdvertisementsDb();
        IMongoCollection<Advertisement> col = db.GetAdvertisementsCol();
        IFindFluent<Advertisement, Advertisement> find = col.Find(filter);
        Advertisement? advertisement = await find.FirstOrDefaultAsync();
        return advertisement == null
            ? Option<Advertisement>.None()
            : Option<Advertisement>.Some(advertisement);
    }

    public async Task<IEnumerable<Advertisement>> GetMany(
        FilterDefinition<Advertisement> filter,
        PaginationOption? pagination = null,
        SortingOption? sorting = null
    ) =>
        await _client
            .GetAdvertisementsDb()
            .GetAdvertisementsCol()
            .Find(filter)
            .When(pagination != null)
            .Apply(find => find.Skip((pagination!.Page - 1) * pagination.PageSize))
            .Apply(find => find.Limit(pagination!.PageSize))
            .Apply(find => find.Accept(ad => ad.Price, sorting))
            .AsList();

    public async Task<long> GetCount(FilterDefinition<Advertisement> filter) =>
        await _client
            .GetAdvertisementsDb()
            .GetAdvertisementsCol()
            .Find(filter)
            .CountDocumentsAsync();
}
