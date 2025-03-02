using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.ParserProfileManagement.CreateProfile;
using RemTechAvito.Application.ParserProfileManagement.DeleteProfile;
using RemTechAvito.Application.ParserProfileManagement.UpdateProfile;
using RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;
using RemTechAvito.DependencyInjection;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Integrational.Tests.Helpers;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Integrational.Tests.ParserProfileManagement;

public sealed class Parser_Profile_CRUD_Tests
{
    private readonly IServiceProvider _provider;
    private readonly ILogger _logger;

    public Parser_Profile_CRUD_Tests()
    {
        IServiceCollection services = new ServiceCollection();
        services.RegisterServices();
        _provider = services.BuildServiceProvider();
        _logger = _provider.GetRequiredService<ILogger>();
    }

    [Fact]
    public async Task Create_Parser_Profile_Test()
    {
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;
        var noExceptions = true;
        try
        {
            var createCommand = new CreateProfileCommand("Test Profile 1");
            var createHandler = _provider.GetRequiredService<
                IAvitoCommandHandler<CreateProfileCommand, ParserProfileResponse>
            >();
            var creation = await createHandler.Handle(createCommand, ct);
            Assert.True(creation.IsSuccess);

            var readRepository = _provider.GetRequiredService<IParserProfileReadRepository>();
            var response = await readRepository.GetById(creation.Value.Id.ToString(), ct);

            Assert.True(response.IsSuccess);
            Assert.Equal(creation.Value.Id, response.Value.Id.Id);
            Assert.Equal(creation.Value.Name, response.Value.Name.Name);
            Assert.Equal(creation.Value.State, response.Value.State.IsActive);
            Assert.Equal(creation.Value.StateDescription, response.Value.State.Description);
            Assert.Empty(response.Value.Links);

            var deleteCommand = new DeleteParserProfileCommand(creation.Value.Id.ToString());
            var deleteHandler = _provider.GetRequiredService<
                IAvitoCommandHandler<DeleteParserProfileCommand>
            >();
            var deletion = await deleteHandler.Handle(deleteCommand, ct);
            Assert.True(deletion.IsSuccess);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.LogTestFailedWithException(nameof(Create_Parser_Profile_Test), ex);
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Update_Parser_Profile_With_Additions_Test()
    {
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;
        var noExceptions = true;
        try
        {
            var createCommand = new CreateProfileCommand("Test Profile 1");
            var createHandler = _provider.GetRequiredService<
                IAvitoCommandHandler<CreateProfileCommand, ParserProfileResponse>
            >();
            var creation = await createHandler.Handle(createCommand, ct);
            Assert.True(creation.IsSuccess);

            var dto = new ParserProfileDto(
                creation.Value.Id.ToString(),
                creation.Value.Name,
                creation.Value.State,
                [
                    new ParserProfileLinkDto("LiuGong", "http://localhost.com", ["H855"]),
                    new ParserProfileLinkDto("ACE", "https://vk.com"),
                ]
            );

            var updateCommand = new UpdateParserProfileCommand(dto);
            var updateHandler = _provider.GetRequiredService<
                IAvitoCommandHandler<UpdateParserProfileCommand>
            >();
            var updateResult = await updateHandler.Handle(updateCommand, ct);
            Assert.True(updateResult.IsSuccess);

            var readRepository = _provider.GetRequiredService<IParserProfileReadRepository>();
            var updatedProfile = await readRepository.GetById(creation.Value.Id.ToString(), ct);
            var withAdditions = updatedProfile.Value.Links.FirstOrDefault(l =>
                l.Unwrap<ParserProfileLinkWithAdditions>().IsSuccess
            )!;
            var withoutAdditions = updatedProfile.Value.Links.FirstOrDefault(l =>
                l.Unwrap<ParserProfileLinkWithAdditions>().IsFailure
            )!;
            Assert.Equal("ACE", withoutAdditions.Name);
            Assert.Equal("https://vk.com", withoutAdditions.Link);
            Assert.Equal("LiuGong", withAdditions.Name);
            Assert.Equal("http://localhost.com", withAdditions.Link);
            var casted = withAdditions.Unwrap<ParserProfileLinkWithAdditions>();
            Assert.True(casted.IsSuccess);
            Assert.NotEmpty(casted.Value.Additions);
            Assert.Contains("H855", casted.Value.Additions);
            Assert.Equal(creation.Value.Name, updatedProfile.Value.Name.Name);
            Assert.Equal(creation.Value.State, updatedProfile.Value.State.IsActive);
            Assert.Equal(creation.Value.StateDescription, updatedProfile.Value.State.Description);
            Assert.Equal(creation.Value.Id, updatedProfile.Value.Id.Id);
            var deleteCommand = new DeleteParserProfileCommand(creation.Value.Id.ToString());
            var deleteHandler = _provider.GetRequiredService<
                IAvitoCommandHandler<DeleteParserProfileCommand>
            >();
            var deletion = await deleteHandler.Handle(deleteCommand, ct);
            Assert.True(deletion.IsSuccess);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.LogTestFailedWithException(
                nameof(Update_Parser_Profile_With_Additions_Test),
                ex
            );
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Update_Parser_Profile_With_Additions_And_Get_As_Response_Test()
    {
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;
        var noExceptions = true;
        try
        {
            var createCommand = new CreateProfileCommand("Test Profile 1");
            var createHandler = _provider.GetRequiredService<
                IAvitoCommandHandler<CreateProfileCommand, ParserProfileResponse>
            >();
            var creation = await createHandler.Handle(createCommand, ct);
            Assert.True(creation.IsSuccess);

            var dto = new ParserProfileDto(
                creation.Value.Id.ToString(),
                creation.Value.Name,
                creation.Value.State,
                [
                    new ParserProfileLinkDto("LiuGong", "http://localhost.com", ["H855"]),
                    new ParserProfileLinkDto("ACE", "https://vk.com"),
                ]
            );

            var updateCommand = new UpdateParserProfileCommand(dto);
            var updateHandler = _provider.GetRequiredService<
                IAvitoCommandHandler<UpdateParserProfileCommand>
            >();
            var updateResult = await updateHandler.Handle(updateCommand, ct);
            Assert.True(updateResult.IsSuccess);

            var readRepository = _provider.GetRequiredService<IParserProfileReadRepository>();
            var updatedProfile = await readRepository.GetById(creation.Value.Id.ToString(), ct);
            var withAdditions = updatedProfile.Value.Links.FirstOrDefault(l =>
                l.Unwrap<ParserProfileLinkWithAdditions>().IsSuccess
            )!;
            var withoutAdditions = updatedProfile.Value.Links.FirstOrDefault(l =>
                l.Unwrap<ParserProfileLinkWithAdditions>().IsFailure
            )!;
            Assert.Equal("ACE", withoutAdditions.Name);
            Assert.Equal("https://vk.com", withoutAdditions.Link);
            Assert.Equal("LiuGong", withAdditions.Name);
            Assert.Equal("http://localhost.com", withAdditions.Link);
            var casted = withAdditions.Unwrap<ParserProfileLinkWithAdditions>();
            Assert.True(casted.IsSuccess);
            Assert.NotEmpty(casted.Value.Additions);
            Assert.Contains("H855", casted.Value.Additions);
            Assert.Equal(creation.Value.Name, updatedProfile.Value.Name.Name);
            Assert.Equal(creation.Value.State, updatedProfile.Value.State.IsActive);
            Assert.Equal(creation.Value.StateDescription, updatedProfile.Value.State.Description);
            Assert.Equal(creation.Value.Id, updatedProfile.Value.Id.Id);

            var response = await readRepository.Get(ct);
            Assert.NotEmpty(response);
            foreach (var item in response)
            {
                var HaswithAdditions = item.Links.Any(l => l.Additions.Any());
                Assert.True(HaswithAdditions);
            }

            var deleteCommand = new DeleteParserProfileCommand(creation.Value.Id.ToString());
            var deleteHandler = _provider.GetRequiredService<
                IAvitoCommandHandler<DeleteParserProfileCommand>
            >();
            var deletion = await deleteHandler.Handle(deleteCommand, ct);
            Assert.True(deletion.IsSuccess);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.LogTestFailedWithException(
                nameof(Update_Parser_Profile_With_Additions_And_Get_As_Response_Test),
                ex
            );
        }

        Assert.True(noExceptions);
    }
}
