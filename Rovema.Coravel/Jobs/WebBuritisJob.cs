using Coravel.Invocable;
using Dclt.Shared.Extensions;
using Microsoft.Playwright;
using Rovema.Shared.Interfaces;
using Rovema.Shared.Models;
using Rovema.Shared.Services;

namespace Rovema.Coravel.Jobs;

public class WebBuritisJob(ILogger<IonJob> logger, PlayService play, IRovemaService rovema) : IInvocable
{
    public async Task Invoke()
    {
        var rpas = (await rovema.GetRpasInverterAsync()).ToArray();

        await play.CreatePages(rpas.Length);

        if (rpas != null)
        {
    
            var tasks = new Task[rpas.Length];
            for (int i = 0; i < rpas.Length; i++)
            {
                var type = rpas[i].Settings.GetKey("type");
                if (string.IsNullOrEmpty(type)) throw new Exception("[user] setting not exists");

                var page = await play.GetPage(i);
                switch (type.ToLower())
                {
                    case "enspire": tasks[i] = ReadEnspire(page, rpas[i]); break;
                }
            }
            await Task.WhenAll(tasks);
            logger.LogInformation($"WebBuritisJob executado às {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
        }
    }

    private async Task ReadEnspire(IPage page, RpaModel rpa)
    {
        var user = rpa.Settings.GetKey("user");
        var password = rpa.Settings.GetKey("password");
        var url = rpa.Settings.GetKey("url");
        var detailUrl = rpa.Settings.GetKey("detailUrl");

        if (string.IsNullOrEmpty(user)) throw new Exception("[user] setting not exists");
        if (string.IsNullOrEmpty(password)) throw new Exception("[password] setting not exists");
        if (string.IsNullOrEmpty(url)) throw new Exception("[url] setting not exists");
        if (string.IsNullOrEmpty(detailUrl)) throw new Exception("[detailUrl] setting not exists");

        if (page.Url.Contains("blank")) // first acces
        {
            await page.GotoAsync(url);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await page.FillAsync("#userName", user);
            await page.FillAsync("#password", password);
            await page.PressAsync("#password", "Enter");
            await page.GotoAsync(detailUrl);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        if (page.Url.ToLower() == detailUrl.ToLower()) // first acces
        {
            // Buscar os valores dos elementos
            var powerValue = await page.TextContentAsync("#pvSystemOverviewPower");
            var powerUnit = await page.TextContentAsync("#pvSystemOverviewPowerUnit");
            var dayValue = await page.TextContentAsync("#overviewDayPower");
            var dayUnit = await page.TextContentAsync("#overviewDayPowerUnit");
            var totalValue = await page.TextContentAsync("#pvSystemTotalPower");
            var totalUnit = await page.TextContentAsync("#pvSystemTotalPowerUnit");

        }

    }

}
