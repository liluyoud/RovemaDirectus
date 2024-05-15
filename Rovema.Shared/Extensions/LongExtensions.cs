namespace Rovema.Shared.Extensions;

public static class LongExtensions
{
    public static DateTime ConvertUnixTimeToDateTime(this long unixTime)
    {
        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dt.AddSeconds(unixTime).ToLocalTime();
    }

    public static DateTime? ConvertUnixTimeToDateTime(this long? unixTime)
    {
        if (unixTime == null) return null;

        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dt.AddSeconds(unixTime.Value).ToLocalTime();
    }
}
