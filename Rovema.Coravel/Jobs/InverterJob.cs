using Coravel.Invocable;
using Dclt.Directus;
using Dclt.Shared.Extensions;
using Microsoft.Playwright;
using Rovema.Shared.Extensions;
using Rovema.Shared.Models;
using Rovema.Shared.Services;

namespace Rovema.Coravel.Jobs;

public class InverterJob(ILogger<InverterJob> logger, DirectusService directusService, PlayService play) : IInvocable
{
    public async Task Invoke()
    {
        var query = new DirectusQuery()
                   .Fields("id,name,type,settings,date_created")
                   .Filter("type", Operation.Equal, "Inversor")
                   .Filter("status", Operation.Equal, "published")
                   .Build();

        var rpas = (await directusService.GetItemsAsync<IEnumerable<RpaModel>>("rpas", query))?.ToArray();

        if (rpas != null)
        {
            int totalRpas = rpas.Count();
            await play.CreatePages(totalRpas);


            var tasks = new Task[totalRpas];
            for (int i = 0; i < totalRpas; i++)
            {
                var type = rpas[i].Settings.GetKey("type");
                if (string.IsNullOrEmpty(type)) throw new Exception("[user] setting not exists");

                var page = await play.GetPage(i);
                switch (type.ToLower())
                {
                    case "enspire": tasks[i] = ReadEnspire(page, rpas[i]); break;
                    case "solarcloud": tasks[i] = ReadSolarCloud(page, rpas[i]); break;
                    case "solarview": tasks[i] = ReadSolarView(page, rpas[i]); break;
                }
            }
            await Task.WhenAll(tasks);
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

    private async Task ReadSolarCloud(IPage page, RpaModel rpa)
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

        if (page.Url.ToLower().Contains("plantlist")) // at plant list
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

    private async Task ReadSolarView(IPage page, RpaModel rpa)
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
        }

        if (page.Url.ToLower() == detailUrl.ToLower())
        {
            await page.ReloadAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Task.Delay(10000);

            var readInverter = new CreateReadInverter() { RpaId = rpa.Id };
            
            var currentElement = await page.QuerySelectorAsync("#textPotenciaInstantanea");
            if (currentElement != null)
            {
                var currentValue = await currentElement.GetAttributeAsync("title");
                readInverter.Current = currentValue.ToDouble(',');
            }

            var monthElement = await page.QuerySelectorAsync("#textEnergia");
            if (monthElement != null)
            {
                var monthValue = await monthElement.GetAttributeAsync("title");
                readInverter.Month = monthValue.ToDouble(',');
            }

            await page.ClickAsync("#tab1");
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Task.Delay(10000);
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

    async Task CheckSkip(IPage page)
    {
        await Task.Delay(10000);
        var skip = await page.QuerySelectorAsync(".introjs-skipbutton");
        if (skip != null)
            await skip.ClickAsync();
    }

}
