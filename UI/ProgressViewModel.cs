using TrayTempra.Models;

namespace TrayTempra.UI;

public sealed record ProgressViewModel(string Label, int Percentage, double Value)
{
    public static ProgressViewModel FromItem(ProgressItem item) => new(item.Label, item.Percentage, item.ClampedValue);
}
