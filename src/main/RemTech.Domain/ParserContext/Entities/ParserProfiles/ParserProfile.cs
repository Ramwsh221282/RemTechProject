using RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;
using RemTech.Domain.ParserContext.ValueObjects;

namespace RemTech.Domain.ParserContext.Entities.ParserProfiles;

public sealed class ParserProfile
{
    public ParserProfileId Id { get; }
    public ParserId ParserId { get; private set; }
    public Parser Parser { get; private set; }
    public ParserProfileName Name { get; private set; }
    public ParserProfileSchedule Schedule { get; private set; }
    public ParserProfileState State { get; private set; }
    public ParserProfileLinksCollection Links { get; private set; }

    private ParserProfile() { } // ef core

    public ParserProfile(Parser parser, ParserProfileName name)
    {
        Id = ParserProfileId.New();
        ParserId = parser.Id;
        Parser = parser;
        Name = name;
        State = ParserProfileState.Disabled;
        Schedule = ParserProfileSchedule.CreateNonSet();
        Links = ParserProfileLinksCollection.Empty();
    }

    public void Update(
        ParserProfileName? newName = null,
        ParserProfileSchedule? schedule = null,
        ParserProfileState? state = null,
        ParserProfileLinksCollection? links = null
    )
    {
        Name = newName ?? Name;
        Schedule = schedule ?? Schedule;
        State = state ?? State;
        Links = links ?? Links;
    }
}
