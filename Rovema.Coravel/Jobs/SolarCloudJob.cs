using Coravel.Invocable;
using Dclt.Directus;
using Dclt.Shared.Extensions;
using Microsoft.Playwright;
using Rovema.Shared.Extensions;
using Rovema.Shared.Models;
using Rovema.Shared.Services;

namespace Rovema.Coravel.Jobs;

public class SolarCloudJob(ILogger<SolarCloudJob> logger, DirectusService directusService, PlayService play) : IInvocable
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
                if (type != null && type.ToLower() == "solarcloud")
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
            await page.GetByPlaceholder("Conta").FillAsync(user);
            await page.GetByPlaceholder("Senha").FillAsync(password);
            await page.GetByText("Entrar").ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await CheckSkip(page);
        }

        if (page.Url.Contains("login")) // login page
        {
            await page.ReloadAsync(); 
            await page.GetByPlaceholder("Conta").FillAsync(user);
            await page.GetByPlaceholder("Senha").FillAsync(password);
            await page.GetByText("Entrar").ClickAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await CheckSkip(page);
        }

        if (page.Url.ToLower().Contains("plantlist")) // at plant list
        {
            await page.GotoAsync(detailUrl);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await CheckSkip(page);
        }

        if (page.Url.ToLower().Contains("dashboard")) // at plant list
        {
            await page.GotoAsync(detailUrl);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await CheckSkip(page);
        }

        if (page.Url.ToLower().Contains("plantdetail")) 
        { 
            var readInverter = new CreateReadInverter() { RpaId = rpa.Id };

            var currentValue = await page.QuerySelectorAsync(".overview-point-value");
            var currentUnit = await page.QuerySelectorAsync(".overview-point-unit");
            if (currentValue != null && currentUnit != null)
            {
                var currentValueStr = await currentValue.InnerTextAsync();
                var currentUnitStr = await currentUnit.InnerTextAsync();
                readInverter.Current = currentValueStr.ToDouble(',') * currentUnitStr.GetPowerMultiplicator();
            }

            var dayValue = await page.QuerySelectorAsync(".item-value");
            var dayUnit = await page.QuerySelectorAsync(".item-unit");
            if (dayValue != null && dayUnit != null)
            {
                var dayValueStr = await dayValue.InnerTextAsync();
                var dayUnitStr = await dayUnit.InnerTextAsync();
                readInverter.Day = dayValueStr.ToDouble(',') * dayUnitStr.GetPowerMultiplicator();
            }

            if (readInverter.Current != null && readInverter.Current > 0)
            {
                await directusService.CreateItemAsync("reads_inverter", readInverter);
                logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - InverterJob {rpa.Name} executado");
            } 
            else
            {
                // trye to refresh page
                await page.ReloadAsync();
            }
        }

    }

    async Task CheckSkip(IPage page)
    {
        await Task.Delay(10000);
        var skip = await page.QuerySelectorAsync(".introjs-skipbutton");
        if (skip != null)
            await skip.ClickAsync();
    }

}
