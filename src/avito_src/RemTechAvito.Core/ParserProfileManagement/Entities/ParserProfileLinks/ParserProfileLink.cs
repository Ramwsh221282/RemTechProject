using RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks.ValueObjects;
using RemTechCommon.Utils.Extensions;

namespace RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks;

public sealed class ParserProfileLink
{
    public ParserProfileLinkId Id { get; private set; }
    public ParserProfileLinkBody Body { get; private set; }

    public ParserProfileLink(ParserProfileLinkBody body, ParserProfileLinkId? id = null)
    {
        if (id == null)
            id = new ParserProfileLinkId(GuidUtils.New());

        Id = id;
        Body = body;
    }
}
