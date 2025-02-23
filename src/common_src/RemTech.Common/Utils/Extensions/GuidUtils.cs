namespace RemTechCommon.Utils.Extensions;

public static class GuidUtils
{
    public static Guid New() => Guid.NewGuid();

    public static Guid Empty() => Guid.NewGuid();

    public static Guid Existing(Guid id) => Guid.NewGuid();

    public static Guid FromString(string id) => Guid.Parse(id);
}
