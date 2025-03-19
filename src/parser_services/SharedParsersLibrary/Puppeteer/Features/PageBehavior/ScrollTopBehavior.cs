using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.PageBehavior;

public sealed class ScrollTopBehavior : IPageBehavior
{
    private const string calculateCurrentHeightScript = "document.body.scrollHeight";
    private const string scrollTopScript = "window.scrollTo(0, 0)";

    public async Task Invoke(IPage page)
    {
        int currentHeight = await CalculateCurrentHeight(page);
        while (true)
        {
            await PerformScrollTop(page, 2);
            int newHeight = await CalculateCurrentHeight(page);

            if (newHeight == currentHeight)
                break;

            currentHeight = newHeight;
        }
    }

    private static async Task<int> CalculateCurrentHeight(IPage page) =>
        await page.EvaluateExpressionAsync<int>(calculateCurrentHeightScript);

    private static async Task PerformScrollTop(IPage page, int delaySeconds)
    {
        await page.EvaluateExpressionAsync(scrollTopScript);
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
    }
}
