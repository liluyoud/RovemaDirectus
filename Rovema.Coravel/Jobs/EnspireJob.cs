using Coravel.Invocable;
using Dclt.Directus;
using Dclt.Shared.Extensions;
using Microsoft.Playwright;
using Rovema.Shared.Extensions;
using Rovema.Shared.Models;
using Rovema.Shared.Services;

namespace Rovema.Coravel.Jobs;

public class EnspireJob(ILogger<EnspireJob> logger, DirectusService directusService, PlayService play) : IInvocable
{
    public async Task Invoke()
    {
        await play.CreatePlay();

        var query = new DirectusQuery()
                   .Fields("id,name,type,settings,date_created")
                   .Filter("type", Operation.Equal, "Inversor")
                   .Filter("status", Operation.Equal, "published")
                   .Build();

        var rpas = (await directusService.GetItemsAsync<IEnumerable<RpaModel>>("rpas", query))?.ToArray();

        if (rpas != null)
        {
            var tasks = new List<Task>();
            foreach (var rpa in rpas)
            {
                var type = rpa.Settings.GetKey("type");
                if (type != null && type.ToLower() == "enspire")
                {
                    var page = await play.GetPage(rpa.Id);
                    tasks.Add(Read(page, rpa));
                }
            }
            await Task.WhenAll(tasks);
        }
    }

    private async Task Read(IPage page, RpaModel rpa)
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
            var currentValue = await page.TextContentAsync("#pvSystemOverviewPower");
            var currentUnit = await page.TextContentAsync("#pvSystemOverviewPowerUnit");
            var dayValue = await page.TextContentAsync("#overviewDayPower");
            var dayUnit = await page.TextContentAsync("#overviewDayPowerUnit");
            var lifetimeValue = await page.TextContentAsync("#pvSystemTotalPower");
            var lifetimeUnit = await page.TextContentAsync("#pvSystemTotalPowerUnit");

            var current = currentValue.ToDouble('.') * currentUnit.GetPowerMultiplicator();
            var day = dayValue.ToDouble('.') * dayUnit.GetPowerMultiplicator();
            var total = lifetimeValue.ToDouble('.') * lifetimeUnit.GetPowerMultiplicator();

            if (current != null && current > 0)
            {
                var readInverter = new CreateReadInverter()
                {
                    RpaId = rpa.Id,
                    Current = current,
                    Day = day,
                    Lifetime = total
                };

                await directusService.CreateItemAsync("reads_inverter", readInverter);
                logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - InverterJob {rpa.Name} executado");
            }
            else
            {
                // try to refresh page
                await page.ReloadAsync();
            }
        }

    }



}
