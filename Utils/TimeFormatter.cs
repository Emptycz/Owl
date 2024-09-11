namespace Utils;

public static class TimeFormatter
{
    public static string ToTimeElapsed(float time)
    {
        string[] units = ["ms", "s", "m", "h"];
        var unitIndex = 0;

        while (time >= 1000 && unitIndex < units.Length - 1)
        {
            time /= 1000;
            unitIndex++;
        }

        return $"{time:n2} {units[unitIndex]}";
    }

    public static string ToTimeElapsed(TimeSpan time)
    {
        string[] units = ["ms", "s", "m", "h"];
        var unitIndex = 0;
        var milliseconds = time.TotalMilliseconds;

        while (milliseconds >= 1000 && unitIndex < units.Length - 1)
        {
            milliseconds /= 1000;
            unitIndex++;
        }

        return $"{milliseconds:n2} {units[unitIndex]}";
    }
}
