using Coravel.Invocable;
using Dclt.Shared.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Playwright;
using Rovema.Shared.Extensions;
using Rovema.Shared.Interfaces;
using Rovema.Shared.Models;
using Rovema.Shared.Services;

namespace Rovema.Coravel.Jobs;

public class InverterJob(ILogger<InverterJob> logger, PlayService play, IRovemaService rovema) : IInvocable
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
                    case "solarcloud": tasks[i] = ReadSolarCloud(page, rpas[i]); break;
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
                var inverter = new CreateReadInverter()
                {
                    RpaId = rpa.Id,
                    Current = current,
                    Day = day,
                    Lifetime = total
                };

                var read = await rovema.AddInverterAsync(inverter);
            }
        }

    }

    private async Task ReadSolarCloud(IPage page, RpaModel rpa)
    {
        var user = rpa.Settings.GetKey("user");
        var password = rpa.Settings.GetKey("password");
        var url = rpa.Settings.GetKey("url");
        var detail = rpa.Settings.GetKey("detail");
        var detailUrl = rpa.Settings.GetKey("detailUrl");

        if (string.IsNullOrEmpty(user)) throw new Exception("[user] setting not exists");
        if (string.IsNullOrEmpty(password)) throw new Exception("[password] setting not exists");
        if (string.IsNullOrEmpty(url)) throw new Exception("[url] setting not exists");
        if (string.IsNullOrEmpty(detail)) throw new Exception("[detail] setting not exists");
        if (string.IsNullOrEmpty(detailUrl)) throw new Exception("[detailUrl] setting not exists");

        if (page.Url.Contains("blank")) // first acces
        {
            await page.GotoAsync(url);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await page.GetByPlaceholder("Conta").FillAsync("ufvsrovema@gmail.com");
            await page.GetByPlaceholder("Senha").FillAsync("rovema123");
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
            var inverter = new CreateReadInverter() { RpaId = rpa.Id };

            var currentValue = await page.QuerySelectorAsync(".overview-point-value");
            var currentUnit = await page.QuerySelectorAsync(".overview-point-unit");
            if (currentValue != null && currentUnit != null)
            {
                var currentValueStr = await currentValue.InnerTextAsync();
                var currentUnitStr = await currentUnit.InnerTextAsync();
                inverter.Current = currentValueStr.ToDouble(',') * currentUnitStr.GetPowerMultiplicator();
            }

            var dayValue = await page.QuerySelectorAsync(".item-value");
            var dayUnit = await page.QuerySelectorAsync(".item-unit");
            if (dayValue != null && dayUnit != null)
            {
                var dayValueStr = await dayValue.InnerTextAsync();
                var dayUnitStr = await dayUnit.InnerTextAsync();
                inverter.Day = dayValueStr.ToDouble(',') * dayUnitStr.GetPowerMultiplicator();
            }

            if (inverter.Current != null && inverter.Current > 0)
            {
                var read = await rovema.AddInverterAsync(inverter);
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
