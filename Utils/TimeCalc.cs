namespace Utils;

public static class TimeCalc
{
    public static string CalculateTime(float time)
    {
        string[] units = { "ms", "s", "m", "h" };
        int unitIndex = 0;

        while (time >= 1000 && unitIndex < units.Length - 1)
        {
            time /= 1000;
            unitIndex++;
        }

        return $"{time:n2} {units[unitIndex]}";
    }
}
