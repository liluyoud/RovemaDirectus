using Microsoft.Extensions.Logging;
using Microsoft.Playwright;


namespace Rovema.Shared.Services;

public class PlayService
{
    private readonly ILogger<PlayService> _logger;

    private IPlaywright? play;
    private IBrowser? browser;
    private List<PageModel> pages = new();

    public PlayService(ILogger<PlayService> logger)
    {
        _logger = logger;
    }

    public async Task CreatePlay()
    {
        if (play == null) // start play and browser
        {
            play = await Playwright.CreateAsync();
            browser = await play.Chromium.LaunchAsync(new()
            {
                Headless = false,
                Channel = "msedge"
            });
        }
    }

    public async Task<IPage> GetPage(int index)
    {
        if (play == null || browser == null ) 
            throw new Exception("You have to call CreatePlay first.");

        var page = pages.FirstOrDefault(p => p.Id == index);

        if (page == null)
        {
            page = new PageModel()
            {
                Id = index,
                Page = await browser.NewPageAsync()
            };
            pages.Add(page);
        }
        else
        {
            if (page.Page == null || page.Page.IsClosed)
            {
                page.Page = await browser.NewPageAsync();
            } 
        }
        return page.Page;
    }
}

public class PageModel
{
    public int Id { get; set; }
    public IPage? Page { get; set; }
}
