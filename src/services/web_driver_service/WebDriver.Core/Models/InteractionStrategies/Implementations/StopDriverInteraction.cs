using System.Diagnostics;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class StopDriverInteraction : IInteractionStrategy
{
    public async Task<Result> Perform(WebDriverInstance instance)
    {
        if (instance.IsDisposed)
            return await Task.FromResult(new Error("Web driver is already stopped"));
        if (instance.Instance == null)
            return await Task.FromResult(new Error("Web driver is already stopped"));

        instance.Instance.Close();
        instance.Instance.Quit();
        instance.Instance.Dispose();
        instance.IsDisposed = true;
        instance.Instance = null;

        try
        {
            Process[] chromeDriverProcesses = Process.GetProcessesByName("chromedriver");
            foreach (Process process in chromeDriverProcesses)
            {
                try
                {
                    process.Kill(true);
                    process.WaitForExit(5000);
                }
                catch (Exception ex)
                {
                    return Result.Failure(
                        new Error($"Error killing chromedriver process {process.Id}: {ex.Message}")
                    );
                }
                finally
                {
                    process.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error(ex.Message));
        }

        if (!string.IsNullOrWhiteSpace(instance.UniqueProfilePath))
            Directory.Delete(instance.UniqueProfilePath, true);

        return await Task.FromResult(Result.Success());
    }
}
