using RemTechAvito.Core.Common.ValueObjects;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;
using RemTechCommon.Utils.Extensions;

namespace RemTechAvito.Core.ParserProfileManagement;

public sealed record ParserProfile
{
    private readonly List<ParserProfileLink> _links = [];

    public IReadOnlyCollection<ParserProfileLink> Links
    {
        get => _links;
        private init
        {
            _links.Clear();
            _links.AddRange(value);
        }
    }

    public ParserProfileId Id { get; private init; }
    public DateCreated CreatedOn { get; private init; }
    public ParserProfileState State { get; private init; }
    public ParserProfileName Name { get; private init; }

    public ParserProfile(ParserProfileName name)
    {
        Id = new ParserProfileId(GuidUtils.New());
        CreatedOn = DateCreated.Current();
        State = ParserProfileState.CreateInactive();
        Name = name;
    }

    public ParserProfile Update(
        ParserProfileName name,
        ParserProfileState state,
        ParserProfileLink[] links
    )
    {
        return this with { Name = name, State = state, Links = links };
    }
}
