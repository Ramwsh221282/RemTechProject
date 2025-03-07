namespace AvitoParser.PDK;

public record PuppeteerLaunchOptions(
    int Width = 1920,
    int Height = 1080,
    int DeviceScale = 1,
    bool Headless = true,
    params string[] Arguments
);
