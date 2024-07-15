using Coravel.Invocable;
using Dclt.Directus;
using Dclt.Shared.Extensions;
using Microsoft.Playwright;
using Rovema.Shared.Extensions;
using Rovema.Shared.Models;
using Rovema.Shared.Services;

namespace Rovema.Coravel.Jobs;

public class SolarViewJob(ILogger<SolarViewJob> logger, DirectusService directusService, PlayService play) : IInvocable
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
                if (type != null && type.ToLower() == "solarview")
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

        if (page.Url.ToLower().Contains("blank")) // first acces
        {
            await page.GotoAsync(url);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        if (page.Url.ToLower().Contains("login")) // login page
        {
            var iframeElement = await page.QuerySelectorAsync("#temp_iframe");
            if (iframeElement != null)
            {
                var iframe = await iframeElement.ContentFrameAsync();
                if (iframe != null)
                {
                    await iframe.FillAsync("#email", user);
                    await iframe.FillAsync("#password", password);
                }
            }
        }

        if (page.Url.ToLower() == "https://my.solarview.com.br/")
        {
            await page.GotoAsync(detailUrl);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Task.Delay(10000);
            await page.ClickAsync("#tab1");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Task.Delay(10000);
        }

        if (page.Url.ToLower() == detailUrl.ToLower())
        {
            //await page.ReloadAsync();
            //await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            //await Task.Delay(10000);

            var readInverter = new CreateReadInverter() { RpaId = rpa.Id };

            var currentElement = await page.QuerySelectorAsync("#textPotenciaInstantanea");
            if (currentElement != null)
            {
                var currentValue = await currentElement.GetAttributeAsync("title");
                readInverter.Current = currentValue.ToDouble(',');
            }

            var dayElement = await page.QuerySelectorAsync("#textEnergia");
            if (dayElement != null)
            {
                var dayValue = await dayElement.GetAttributeAsync("title");
                readInverter.Day = dayValue.ToDouble(',');
            }

            if (readInverter.Current != null && readInverter.Current > 0)
            {
                await directusService.CreateItemAsync("reads_inverter", readInverter);
                logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - InverterJob {rpa.Name} executado");
            }
        }
    }
}
