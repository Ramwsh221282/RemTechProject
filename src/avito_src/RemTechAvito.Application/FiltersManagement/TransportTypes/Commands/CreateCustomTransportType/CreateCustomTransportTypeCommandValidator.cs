using FluentValidation;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechCommon.Injections;

namespace RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.CreateCustomTransportType;

internal sealed class CreateCustomTransportTypeCommandValidator
    : AbstractValidator<CreateCustomTransportTypeCommand>
{
    public CreateCustomTransportTypeCommandValidator()
    {
        RuleFor(x => new { x.Name, Text = x.Link })
            .MustBeSuccessResult(obj =>
                UserTransportType.Create(obj.Name, obj.Text, DateOnly.FromDateTime(DateTime.Now))
            );
    }
}
