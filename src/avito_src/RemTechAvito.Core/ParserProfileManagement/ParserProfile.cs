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

    public ParserProfileId Id { get; private set; }
    public DateCreated CreatedOn { get; private set; }
    public ParserProfileState State { get; private set; }

    public ParserProfile()
    {
        Id = new ParserProfileId(GuidUtils.New());
        CreatedOn = DateCreated.Current();
        State = ParserProfileState.CreateInactive();
    }

    public void UpdateProfileLinks(List<ParserProfileLink> links) => Links = links;

    public void UpdateState(ParserProfileState state) => State = state;
}
