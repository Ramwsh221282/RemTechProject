using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.ParserProfileManagement.CreateProfile;
using RemTechAvito.Application.ParserProfileManagement.UpdateProfile;
using RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.ParseTransportAdvertisementsCatalogue;
using RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Integrational.Tests.Helpers;
using WebDriver.Worker.Service.Contracts.BaseContracts;

namespace RemTechAvito.Integrational.Tests.BaseCatalogueParsingTest;

public sealed class BaseCatalogueParsingTests : BasicParserTests
{
    [Fact]
    public async Task Invoke_Parse_Advertisements_Catalogue_Use_Case()
    {
        var noException = true;
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        try
        {
            const string url =
                "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/ace-ASgBAgICAkRU4E3cxg380GY";

            ParseTransportAdvertisementCatalogueCommand command = new(url);
            var handler = _serviceProvider.GetRequiredService<
                IAvitoCommandHandler<ParseTransportAdvertisementCatalogueCommand>
            >();

            var sw = new Stopwatch();
            sw.Start();
            await handler.Handle(command, ct);
            sw.Stop();
            _logger.Information("Time elapsed: {Time}", sw.Elapsed.Minutes);
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "{Test} stopped. Exception: {Ex}",
                nameof(Invoke_Parse_Advertisements_Catalogue_Use_Case),
                ex.Message
            );
            noException = false;
            var stopper = new SingleCommunicationPublisher(queue, host, user, password);
            await stopper.Send(new StopWebDriverContract(), ct);
        }

        Assert.True(noException);
    }

    [Fact]
    public async Task Create_Parser_Profile_And_Invoke_Parsing()
    {
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;
        var noExceptions = true;

        try
        {
            const string catalogueUrl =
                "https://www.avito.ru/all/gruzoviki_i_spetstehnika/tehnika_dlya_lesozagotovki/john_deere-ASgBAgICAkRUsiyexw3W6j8?cd=1";
            var createProfile = new CreateProfileCommand("LuGong 855H");
            var createProfileHandler = _serviceProvider.GetRequiredService<
                IAvitoCommandHandler<CreateProfileCommand, ParserProfileResponse>
            >();
            var createdProfile = await createProfileHandler.Handle(createProfile, ct);
            Assert.True(createdProfile.IsSuccess);

            var updateDto = new ParserProfileDto(
                createdProfile.Value.Id.ToString(),
                createdProfile.Value.Name,
                createdProfile.Value.State,
                [new ParserProfileLinkDto("LiuGong H855", catalogueUrl)]
            );
            var updateProfileCommand = new UpdateParserProfileCommand(updateDto);
            var updateHandler = _serviceProvider.GetRequiredService<
                IAvitoCommandHandler<UpdateParserProfileCommand>
            >();
            var updateResult = await updateHandler.Handle(updateProfileCommand, ct);
            Assert.True(updateResult.IsSuccess);

            var profilesReadRepository =
                _serviceProvider.GetRequiredService<IParserProfileReadRepository>();
            var profile = await profilesReadRepository.GetById(createdProfile.Value.Id.ToString());
            Assert.True(profile.IsSuccess);
            Assert.NotEmpty(profile.Value.Links);

            var transportAdvertisementsReadRepository =
                _serviceProvider.GetRequiredService<ITransportAdvertisementsQueryRepository>();
            var identifiers = await transportAdvertisementsReadRepository.GetAdvertisementIDs(ct);

            foreach (var link in profile.Value.Links)
            {
                IEnumerable<string> additions = [];
                var canUnwrap = link.Unwrap<ParserProfileLinkWithAdditions>();
                if (canUnwrap.IsSuccess)
                    additions = canUnwrap.Value.Additions;

                var parseCommand = new ParseTransportAdvertisementCatalogueCommand(
                    link.Link,
                    identifiers,
                    additions
                );
                var parseHandler = _serviceProvider.GetRequiredService<
                    IAvitoCommandHandler<ParseTransportAdvertisementCatalogueCommand>
                >();
                var result = await parseHandler.Handle(parseCommand, ct);
                Assert.True(result.IsSuccess);
            }

            var data = await transportAdvertisementsReadRepository.Query(
                new GetAnalyticsItemsRequest(1, 50),
                ct
            );
            Assert.NotEmpty(data);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.LogTestFailedWithException(
                nameof(Create_Parser_Profile_And_Invoke_Parsing),
                ex
            );
            var pub = new SingleCommunicationPublisher(queue, host, user, password);
            await pub.Send(new StopWebDriverContract());
        }

        Assert.True(noExceptions);
    }
}
