namespace Rovema.Shared.Models;

public record RpaModel(int Id, string Name, string Type, List<RpaSetting>? Settings)
{
    public string? GetSetting(string key)
    {
        var setting = Settings?.FirstOrDefault(s => s.Key.ToLower() == key.ToLower());
        if (setting != null)
        {
            return setting.Value;
        }
        return null;
    }
}

public record RpaSetting (string Key, string Value);
