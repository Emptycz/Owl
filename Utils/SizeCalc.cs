using System.Text;

namespace Utils;

public static class SizeCalc
{
    public static SizeResponse CalculateSize(string content, Encoding encoding)
    {
        return CalculateSize(encoding.GetByteCount(content));
    }

    public static SizeResponse CalculateSize(int size)
    {
        string[] units = { "B", "KB", "MB", "GB", "TB" };
        int unitIndex = 0;
        double sizeInUnits = size;

        while (sizeInUnits >= 1024 && unitIndex < units.Length - 1)
        {
            sizeInUnits /= 1024;
            unitIndex++;
        }

        return new SizeResponse
        {
            Size = sizeInUnits,
            Unit = units[unitIndex]
        };
    }
}

public class SizeResponse
{
    public required double Size { get; set; }
    public required string Unit { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Size:F2} {Unit}";
    }
}
