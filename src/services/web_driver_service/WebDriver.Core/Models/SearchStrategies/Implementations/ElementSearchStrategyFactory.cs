namespace WebDriver.Core.Models.SearchStrategies.Implementations;

public static class ElementSearchStrategyFactory
{
    public static ISingleElementSearchStrategy CreateForNew(string path, string type) =>
        new NewSingleElementSearchStrategy(type, path);

    public static ISingleElementSearchStrategy CreateForExisting(Guid existingId) =>
        new ExistingElementSearchStrategy(existingId);

    public static ISingleElementSearchStrategy CreateForChild(
        Guid existingId,
        string path,
        string type
    ) => new NewSingleChildElementSearchStrategy(existingId, type, path);

    public static IMultipleElementSearchStrategy CreateForMultipleChilds(
        Guid existingId,
        string path,
        string type
    ) => new NewMultipleChildsStrategy(existingId, path, type);
}
