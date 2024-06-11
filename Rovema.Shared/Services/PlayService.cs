using Microsoft.Extensions.Logging;
using Microsoft.Playwright;


namespace Rovema.Shared.Services;

public class PlayService
{
    private readonly ILogger<PlayService> _logger;

    private IPlaywright? play;
    private IBrowser? browser;
    private IPage[]? pages;

    public PlayService(ILogger<PlayService> logger)
    {
        _logger = logger;   
    }

    public async Task CreatePages(int length)
    {
        if (play == null) // start play and browser
        {
            play = await Playwright.CreateAsync();
            browser = await play.Chromium.LaunchAsync(new()
            {
                Headless = false,
                Channel = "msedge"
            });
            pages = new IPage[length];
        }
    }

    public async Task<IPage> GetPage(int index)
    {
        if (play == null || browser == null || pages == null) 
            throw new Exception("You have to call CreatePages first.");

        if (pages[index] == null)
        {
            pages[index] = await browser.NewPageAsync();
        }

        return pages[index];
    }
}
