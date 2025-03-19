using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.PageBehavior;

public sealed class ScrollBottomBehavior : IPageBehavior
{
    private const string getCurrentHeightScript = "document.body.scrollHeight";
    private const string scrollBottomScript = "window.scrollTo(0, document.body.scrollHeight)";

    public async Task Invoke(IPage page)
    {
        int currentHeight = await CalculateCurrentDocumentHeight(page);
        while (true)
        {
            await ProcessScrollBottom(page, 2);
            int newHeight = await CalculateCurrentDocumentHeight(page);

            if (newHeight == currentHeight)
                break;

            currentHeight = newHeight;
        }
    }

    private static async Task ProcessScrollBottom(IPage page, int delaySeconds)
    {
        await page.EvaluateExpressionAsync(scrollBottomScript);
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
    }

    private static async Task<int> CalculateCurrentDocumentHeight(IPage page) =>
        await page.EvaluateExpressionAsync<int>(getCurrentHeightScript);
}
