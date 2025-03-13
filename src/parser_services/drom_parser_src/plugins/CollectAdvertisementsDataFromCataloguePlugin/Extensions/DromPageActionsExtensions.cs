using CollectAdvertisementsDataFromCataloguePlugin.Models;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using Serilog;

namespace CollectAdvertisementsDataFromCataloguePlugin.Extensions;

public static class DromPageActionsExtensions
{
    public static async Task ScrollPageUp(
        this DromCataloguePage cataloguePage,
        PluginExecutionContext context,
        ILogger logger
    )
    {
        IPage pageValue = cataloguePage.Page;
        PluginPayload payload = new PluginPayload(pageValue, logger);
        await context.Execute("ScrollTopPlugin", payload);
    }

    public static async Task ScrollPageDown(
        this DromCataloguePage cataloguePage,
        PluginExecutionContext context,
        ILogger logger
    )
    {
        IPage pageValue = cataloguePage.Page;
        PluginPayload payload = new PluginPayload(pageValue, logger);
        await context.Execute("ScrollBottomPlugin", payload);
    }
}
