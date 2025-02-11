using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.SearchStrategies;

internal abstract class BaseSearchElementStrategy
{
    private readonly ElementSearchTypeConverter _converter = new();

    private const int MaxInteractionAttemptsCount = 10;
    private const int DelayInSeconds = 5;

    protected Result<By> Convert(string type, string path) => _converter.Convert(type, path);

    protected async Task Wait() => await Task.Delay(TimeSpan.FromSeconds(DelayInSeconds));

    protected bool ReachedMaxAttempts(ref int current) => current < MaxInteractionAttemptsCount;
}
