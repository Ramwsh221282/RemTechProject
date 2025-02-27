using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.ParserProfileManagement.CreateProfile;
using RemTechAvito.Application.ParserProfileManagement.DeleteProfile;
using RemTechAvito.Application.ParserProfileManagement.UpdateParserProfileLinks;
using RemTechAvito.Contracts.Common.Dto.ParserProfileManagement;
using RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;
using RemTechAvito.DependencyInjection;
using RemTechAvito.Infrastructure.Contracts.Repository;

namespace RemTechAvito.Integrational.Tests.ParserProfileManagement;

public sealed class Parser_Profile_CRUD_Tests
{
    private readonly IServiceProvider _provider;
    private const string temporaryId = "1ebd871e-7636-4af5-b20d-2a0dd2419f23";

    public Parser_Profile_CRUD_Tests()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.RegisterServices();
        _provider = collection.BuildServiceProvider();
    }

    [Fact]
    public async Task Create_New_Profile()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var command = new CreateProfileCommand(new ProfileNameDto("test"));
        var handler = _provider.GetRequiredService<
            IAvitoCommandHandler<CreateProfileCommand, ParserProfileResponse>
        >();
        var creation = await handler.Handle(command, token);
        Assert.True(creation.IsSuccess);
    }

    [Fact]
    public async Task Get_Profile_By_ID()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var repository = _provider.GetRequiredService<IParserProfileReadRepository>();
        var profile = await repository.GetById(temporaryId, token);
        Assert.True(profile.IsSuccess);
    }

    [Fact]
    public async Task Update_Profile_Links()
    {
        const string link =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/liugong-ASgBAgICAkRU4E3cxg3urj8?cd=1";
        const string mark = "liugong";
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var handler = _provider.GetRequiredService<
            IAvitoCommandHandler<UpdateParserProfileLinksCommand>
        >();
        var dto = new ParserProfileDto(temporaryId, false, [new ParserProfileLinkDto(mark, link)]);
        var command = new UpdateParserProfileLinksCommand(dto);
        var update = await handler.Handle(command, token);
        Assert.True(update.IsSuccess);
    }

    [Fact]
    public async Task Update_Profile_Links_With_Existing_Links()
    {
        const string existingLink =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/liugong-ASgBAgICAkRU4E3cxg3urj8?cd=1";
        const string existingMark = "liugong";
        const string existingId = "7748e966-a548-4e8c-a031-b929636314bf";

        const string link =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/newmark-ASgBAgICAkRU4E3cxg3urj8?cd=1";
        const string mark = "newmark";
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var handler = _provider.GetRequiredService<
            IAvitoCommandHandler<UpdateParserProfileLinksCommand>
        >();
        var dto = new ParserProfileDto(
            temporaryId,
            false,
            [
                new ParserProfileLinkDto(existingMark, existingLink, existingId),
                new ParserProfileLinkDto(mark, link),
            ]
        );
        var command = new UpdateParserProfileLinksCommand(dto);
        var update = await handler.Handle(command, token);
        Assert.True(update.IsSuccess);
    }

    [Fact]
    public async Task Get_Profiles()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var repository = _provider.GetRequiredService<IParserProfileReadRepository>();
        var data = await repository.Get(token);
        Assert.NotEmpty(data);
    }

    [Fact]
    public async Task Delete_Profile_Fake_ID()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var handler = _provider.GetRequiredService<
            IAvitoCommandHandler<DeleteParserProfileCommand>
        >();
        var command = new DeleteParserProfileCommand("7748e966-a548-4e8c-a031-b929636314bf");
        var update = await handler.Handle(command, token);
        Assert.False(update.IsSuccess);
    }

    [Fact]
    public async Task Delete_Profile_Real_ID()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var handler = _provider.GetRequiredService<
            IAvitoCommandHandler<DeleteParserProfileCommand>
        >();
        var command = new DeleteParserProfileCommand(temporaryId);
        var update = await handler.Handle(command, token);
        Assert.True(update.IsSuccess);
    }

    [Fact]
    public async Task Get_Active_Only_Profiles()
    {
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var repository = _provider.GetRequiredService<IParserProfileReadRepository>();
        var data = await repository.GetActiveOnly(token);
        Assert.NotEmpty(data);
    }
}
