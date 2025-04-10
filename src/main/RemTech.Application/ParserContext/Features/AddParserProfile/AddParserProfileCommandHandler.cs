﻿using RemTech.Application.ParserContext.Contracts;
using RemTech.Domain.ParserContext;
using RemTech.Domain.ParserContext.Entities.ParserProfiles;
using RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;

namespace RemTech.Application.ParserContext.Features.AddParserProfile;

public sealed class AddParserProfileCommandHandler(IParserWriteRepository repository)
    : ICommandHandler<AddParserProfileCommand, UnitResult<ParserProfile>>
{
    private readonly IParserWriteRepository _repository = repository;

    public async Task<UnitResult<ParserProfile>> Handle(
        AddParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        (string parserName, string profileName) = command;
        Option<Parser> parser = await _repository.GetByName(parserName, ct);

        if (parser.HasValue == false)
            return UnitResult<ParserProfile>.FromFailure(
                new Error($"Не найден парсер с названием: {parserName}"),
                UnitResultCodes.NotFound
            );

        ParserProfile profile = new(parser.Value, ParserProfileName.Create(profileName));
        Result adding = parser.Value.AddProfile(profile);

        if (adding.IsFailure)
            return UnitResult<ParserProfile>.FromFailure(adding.Error, UnitResultCodes.BadRequest);

        await _repository.Save(parser.Value, ct);
        return profile;
    }
}
