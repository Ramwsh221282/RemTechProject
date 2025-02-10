using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.SearchStrategies.Implementations;

internal sealed class ExistingElementSearchStrategy : ISingleElementSearchStrategy
{
    private readonly Guid _existingId;

    public ExistingElementSearchStrategy(Guid existingId) => _existingId = existingId;

    public async Task<Result<WebElementObject>> Search(WebDriverInstance instance) =>
        await Task.FromResult(instance.GetFromPool(_existingId));
}
