using FluentValidation;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;

namespace RemTechAvito.Application.ParserProfileManagement.UpdateParserProfileLinks;

internal sealed class UpdateParserProfileLinksCommandValidator
    : AbstractValidator<UpdateParserProfileCommand>
{
    public UpdateParserProfileLinksCommandValidator()
    {
        RuleFor(com => com.Dto)
            .Custom(
                (dto, context) =>
                {
                    var nameResult = ParserProfileName.Create(dto.Name);
                    if (nameResult.IsFailure)
                    {
                        context.AddFailure(nameResult.Error.Description);
                        return;
                    }

                    foreach (var link in dto.Links)
                    {
                        var linkResult = ParserProfileLinkFactory.Create(
                            link.Mark,
                            link.Link,
                            link.Additions
                        );
                        if (!linkResult.IsFailure)
                            continue;
                        context.AddFailure(linkResult.Error.Description);
                        return;
                    }
                }
            );
    }
}
