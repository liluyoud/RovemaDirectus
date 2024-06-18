using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using System.Net;
using Dclt.Shared.Models;
using Dclt.Shared.Extensions;
using Rovema.Shared.Contracts;

namespace Rovema.Shared.Services;

public class ReadService(ILogger<ReadService> logger)
{
    public async Task<IonModel?> GetIonAsync(string address, bool reverse = false)
    {
        try
        {
            var url = $"http://{address}/Operation.html";
            var web = new HtmlWeb();

            web.PreRequest = delegate (HttpWebRequest webRequest)
            {
                webRequest.Timeout = 20000;
                return true;
            };

            var html = await web.LoadFromWebAsync(url);

            var str_Power_KW_Total = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[3]/td[8]").InnerText;
            var str_Power_KW_A = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[4]/td[8]").InnerText;
            var str_Power_KW_B = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[5]/td[8]").InnerText;
            var str_Power_KW_C = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[6]/td[8]").InnerText;

            var str_Power_KVA_Total = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[7]/td[8]").InnerText;
            var str_Power_KVA_A = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[8]/td[8]").InnerText;
            var str_Power_KVA_B = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[9]/td[8]").InnerText;
            var str_Power_KVA_C = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[10]/td[6]").InnerText;

            var str_Power_KVAR_Total = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[11]/td[8]").InnerText;
            var str_Power_KVAR_A = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[12]/td[8]").InnerText;
            var str_Power_KVAR_B = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[13]/td[6]").InnerText;
            var str_Power_KVAR_C = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[14]/td[8]").InnerText;

            var str_Current_I_AVG = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[3]/td[5]").InnerText;
            var str_Current_I_A = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[4]/td[5]").InnerText;
            var str_Current_I_B = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[5]/td[5]").InnerText;
            var str_Current_I_C = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[6]/td[5]").InnerText;
            var str_Current_I_Unbal = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[8]/td[5]").InnerText;

            var str_PowerFactor_Sign_Total = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[11]/td[5]").InnerText;
            var str_PowerFactor_Sign_A = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[12]/td[5]").InnerText;
            var str_PowerFactor_Sign_B = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[13]/td[3]").InnerText;
            var str_PowerFactor_Sign_C = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[14]/td[5]").InnerText;

            var str_Voltage_Vln_Avg = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[3]/td[2]").InnerText;
            var str_Voltage_Vln_A = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[4]/td[2]").InnerText;
            var str_Voltage_Vln_B = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[5]/td[2]").InnerText;
            var str_Voltage_Vln_C = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[6]/td[2]").InnerText;

            var str_Voltage_Vll_Avg = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[7]/td[2]").InnerText;
            var str_Voltage_Vll_AB = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[8]/td[2]").InnerText;
            var str_Voltage_Vll_BC = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[9]/td[2]").InnerText;
            var str_Voltage_Vll_CA = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[10]/td[2]").InnerText;
            var str_Voltage_V_Unbal = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[11]/td[2]").InnerText;

            var str_Frequency = html.DocumentNode.SelectSingleNode("/html[1]/body[1]/table[1]/tr[14]/td[2]").InnerText;
            var multiply = reverse ? -1 : 1;
            var ion = new IonModel()
            {
                Power_KW_Total = str_Power_KW_Total.ToDouble('.', true) * multiply,
                Power_KW_A = str_Power_KW_A.ToDouble('.', true) * multiply,
                Power_KW_B = str_Power_KW_B.ToDouble('.', true) * multiply,
                Power_KW_C = str_Power_KW_C.ToDouble('.', true) * multiply,

                Power_KVA_Total = str_Power_KVA_Total.ToDouble('.'),
                Power_KVA_A = str_Power_KVA_A.ToDouble('.'),
                Power_KVA_B = str_Power_KVA_B.ToDouble('.'),
                Power_KVA_C = str_Power_KVA_C.ToDouble('.'),

                Power_KVAR_Total = str_Power_KVAR_Total.ToDouble('.'),
                Power_KVAR_A = str_Power_KVAR_A.ToDouble('.'),
                Power_KVAR_B = str_Power_KVAR_B.ToDouble('.'),
                Power_KVAR_C = str_Power_KVAR_C.ToDouble('.'),

                Current_I_AVG = str_Current_I_AVG.ToDouble('.'),
                Current_I_A = str_Current_I_A.ToDouble('.'),
                Current_I_B = str_Current_I_B.ToDouble('.'),
                Current_I_C = str_Current_I_C.ToDouble('.'),
                Current_I_Unbal = str_Current_I_Unbal.ToDouble('.'),

                Factor_Sign_Total = str_PowerFactor_Sign_Total.ToDouble('.'),
                Factor_Sign_A = str_PowerFactor_Sign_A.ToDouble('.'),
                Factor_Sign_B = str_PowerFactor_Sign_B.ToDouble('.'),
                Factor_Sign_C = str_PowerFactor_Sign_C.ToDouble('.'),

                Voltage_Vln_Avg = str_Voltage_Vln_Avg.ToDouble('.'),
                Voltage_Vln_A = str_Voltage_Vln_A.ToDouble('.'),
                Voltage_Vln_B = str_Voltage_Vln_B.ToDouble('.'),
                Voltage_Vln_C = str_Voltage_Vln_C.ToDouble('.'),

                Voltage_Vll_Avg = str_Voltage_Vll_Avg.ToDouble('.'),
                Voltage_Vll_AB = str_Voltage_Vll_AB.ToDouble('.'),
                Voltage_Vll_BC = str_Voltage_Vll_BC.ToDouble('.'),
                Voltage_Vll_CA = str_Voltage_Vll_CA.ToDouble('.'),
                Voltage_V_Unbal = str_Voltage_V_Unbal.ToDouble('.'),

                Frequency = str_Frequency.ToDouble('.')
            };

            return ion;
        }
        catch (Exception ex)
        {
            logger.LogError($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - Ion RPA {address}: {ex.Message}");
        }
        return null;
    }

    public async Task<List<KeyValue>?> GetSolarAsync(string address)
    {
        try
        {
            var url = $"http://{address}/channels.php";
            var web = new HtmlWeb();
            web.PreRequest = delegate (HttpWebRequest webRequest)
            {
                webRequest.Timeout = 20000;
                return true;
            };

            var html = await web.LoadFromWebAsync(url);
            var table = html.DocumentNode.SelectSingleNode("//table");
            var rows = table.SelectNodes("//tr").Skip(1);
            var values = new List<KeyValue>();

            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td");
                KeyValue value = new(cells[1].InnerText.Trim(), cells[2].InnerText.Trim());
                values.Add(value);
            }
            return values;
        }
        catch (Exception ex)
        {
            logger.LogError($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - Solar RPA {address}: {ex.Message}");
        }
        return default;
    }

}

