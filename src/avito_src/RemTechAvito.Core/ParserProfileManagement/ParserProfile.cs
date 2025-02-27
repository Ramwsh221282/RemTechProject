using RemTechAvito.Core.Common.ValueObjects;
using RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;
using RemTechCommon.Utils.Extensions;

namespace RemTechAvito.Core.ParserProfileManagement;

public sealed class ParserProfile
{
    private readonly List<ParserProfileLink> _links = [];

    public IReadOnlyCollection<ParserProfileLink> Links
    {
        get => _links;
        private set
        {
            // Bson Class Map
            _links.Clear();
            _links.AddRange(value);
        }
    }

    public ParserProfileId Id { get; private set; } = new(GuidUtils.New());
    public DateCreated CreatedOn { get; private set; } = DateCreated.Current();
    public ParserProfileState State { get; private set; } = ParserProfileState.CreateInactive();
    public ParserProfileName Name { get; private set; }

    public ParserProfile(ParserProfileName name)
    {
        Name = name;
    }

    public ParserProfile Update(ParserProfileName name, List<ParserProfileLink> links, bool state)
    {
        return new ParserProfile(name)
        {
            Links = links,
            State = state ? ParserProfileState.CreateActive() : ParserProfileState.CreateInactive(),
            Id = Id,
            CreatedOn = CreatedOn,
        };
    }
}
