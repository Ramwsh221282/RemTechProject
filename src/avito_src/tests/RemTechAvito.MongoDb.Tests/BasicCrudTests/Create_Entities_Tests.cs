using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;
using RemTechAvito.Core.Common.ValueObjects;
using RemTechAvito.DependencyInjection;
using RemTechAvito.Infrastructure.Contracts.Repository;
using Serilog;
using Characteristic = RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects.Characteristic;

namespace RemTechAvito.MongoDb.Tests.BasicCrudTests;

public class Create_Entities_Tests
{
    private readonly ITransportAdvertisementsCommandRepository _repository;
    private readonly ILogger _logger;

    public Create_Entities_Tests()
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.RegisterServices();
        var provider = serviceCollection.BuildServiceProvider();
        _repository = provider.GetRequiredService<ITransportAdvertisementsCommandRepository>();
        _logger = provider.GetRequiredService<ILogger>();
    }

    [Fact]
    public async Task Insert_Advertisement_Test()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        bool noExceptions = true;

        try
        {
            AdvertisementID id = AdvertisementID.Create("4516615049");
            Characteristics characteristics = new Characteristics(
                [Characteristic.Create("Марка", "Kandela"), Characteristic.Create("Модель", "A-05")]
            );
            Address address = Address.Create(
                "Республика Татарстан, Казань, Вахитовский район, ул. Татарстан"
            );
            OwnerInformation ownerInformation = OwnerInformation.Create("wwwKandelaRu", "Компания");
            Price price = Price.Create("290000", "RUB", "НДС");
            Title title = Title.Create("Вилочный погрузчик Kandela A-05, 2024");
            Description description = Description.Create(
                "Спецтехника КАNDЕLА маневренная, легко перемещается в узких проходах между стеллажами, лучший вариант для небольшого склада!"
            );
            AdvertisementUrl url = AdvertisementUrl.Create(
                "https://www.avito.ru/kazan/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_kandela_a-05_2024_4516615049?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiJ1cEVoQ1hzdGdpUDBhZjUwIjt9fE_bHz8AAAA"
            );
            DateOnly createdOn = DateOnly.FromDateTime(DateTime.Now);
            PhotoAttachments photos = new PhotoAttachments(
                [Photo.Create("url123"), Photo.Create("url456")]
            );
            TransportAdvertisement advertisement = new TransportAdvertisement(
                id,
                characteristics,
                address,
                ownerInformation,
                photos,
                price,
                title,
                description,
                createdOn,
                url
            );
            Guid insertedId = await _repository.Add(advertisement, ct);
            Assert.NotEqual(Guid.Empty, insertedId);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal("{Test} {Ex}", nameof(Insert_Advertisement_Test), ex.Message);
        }
        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Insert_Two_Advertisements_Different_ID_Test()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        bool noExceptions = true;
        try
        {
            AdvertisementID id = AdvertisementID.Create("4516615049");
            AdvertisementID id2 = AdvertisementID.Create("5613655449");

            Characteristics characteristics = new Characteristics(
                [Characteristic.Create("Марка", "Kandela"), Characteristic.Create("Модель", "A-05")]
            );
            Address address = Address.Create(
                "Республика Татарстан, Казань, Вахитовский район, ул. Татарстан"
            );
            OwnerInformation ownerInformation = OwnerInformation.Create("wwwKandelaRu", "Компания");
            Price price = Price.Create("290000", "RUB", "НДС");
            Title title = Title.Create("Вилочный погрузчик Kandela A-05, 2024");
            Description description = Description.Create(
                "Спецтехника КАNDЕLА маневренная, легко перемещается в узких проходах между стеллажами, лучший вариант для небольшого склада!"
            );
            AdvertisementUrl url = AdvertisementUrl.Create(
                "https://www.avito.ru/kazan/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_kandela_a-05_2024_4516615049?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiJ1cEVoQ1hzdGdpUDBhZjUwIjt9fE_bHz8AAAA"
            );
            DateOnly createdOn = DateOnly.FromDateTime(DateTime.Now);
            PhotoAttachments photos = new PhotoAttachments(
                [Photo.Create("url123"), Photo.Create("url456")]
            );
            TransportAdvertisement advertisement1 = new TransportAdvertisement(
                id,
                characteristics,
                address,
                ownerInformation,
                photos,
                price,
                title,
                description,
                createdOn,
                url
            );
            TransportAdvertisement advertisement2 = new TransportAdvertisement(
                id2,
                characteristics,
                address,
                ownerInformation,
                photos,
                price,
                title,
                description,
                createdOn,
                url
            );
            Guid insertedId = await _repository.Add(advertisement1, ct);
            Assert.NotEqual(Guid.Empty, insertedId);
            insertedId = await _repository.Add(advertisement2, ct);
            Assert.NotEqual(Guid.Empty, insertedId);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal("{Test} {Ex}", nameof(Insert_Advertisement_Test), ex.Message);
        }
        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Insert_Two_Advertisement_Same_ID_Test()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        bool noExceptions = true;

        try
        {
            AdvertisementID id = AdvertisementID.Create("4516615049");
            AdvertisementID id2 = AdvertisementID.Create("4516615049");

            Characteristics characteristics = new Characteristics(
                [Characteristic.Create("Марка", "Kandela"), Characteristic.Create("Модель", "A-05")]
            );
            Address address = Address.Create(
                "Республика Татарстан, Казань, Вахитовский район, ул. Татарстан"
            );
            OwnerInformation ownerInformation = OwnerInformation.Create("wwwKandelaRu", "Компания");
            Price price = Price.Create("290000", "RUB", "НДС");
            Title title = Title.Create("Вилочный погрузчик Kandela A-05, 2024");
            Description description = Description.Create(
                "Спецтехника КАNDЕLА маневренная, легко перемещается в узких проходах между стеллажами, лучший вариант для небольшого склада!"
            );
            AdvertisementUrl url = AdvertisementUrl.Create(
                "https://www.avito.ru/kazan/gruzoviki_i_spetstehnika/vilochnyy_pogruzchik_kandela_a-05_2024_4516615049?context=H4sIAAAAAAAA_wE_AMD_YToyOntzOjEzOiJsb2NhbFByaW9yaXR5IjtiOjA7czoxOiJ4IjtzOjE2OiJ1cEVoQ1hzdGdpUDBhZjUwIjt9fE_bHz8AAAA"
            );
            DateOnly createdOn = DateOnly.FromDateTime(DateTime.Now);
            PhotoAttachments photos = new PhotoAttachments(
                [Photo.Create("url123"), Photo.Create("url456")]
            );
            TransportAdvertisement advertisement1 = new TransportAdvertisement(
                id,
                characteristics,
                address,
                ownerInformation,
                photos,
                price,
                title,
                description,
                createdOn,
                url
            );
            TransportAdvertisement advertisement2 = new TransportAdvertisement(
                id2,
                characteristics,
                address,
                ownerInformation,
                photos,
                price,
                title,
                description,
                createdOn,
                url
            );
            Guid insertedId = await _repository.Add(advertisement1, ct);
            Assert.NotEqual(Guid.Empty, insertedId);
            insertedId = await _repository.Add(advertisement2, ct);
            Assert.Equal(Guid.Empty, insertedId);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal("{Test} {Ex}", nameof(Insert_Advertisement_Test), ex.Message);
        }

        Assert.True(noExceptions);
    }
}
