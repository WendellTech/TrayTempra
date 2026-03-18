namespace TrayTempra.Models;

public readonly record struct ProgressItem(string Label, double Value, bool Enabled = true)
{
    public double ClampedValue => Math.Clamp(Value, 0, 1);
    public int Percentage => (int)Math.Round(ClampedValue * 100, MidpointRounding.AwayFromZero);
}

public readonly record struct ProgressSnapshot(
    DateTime Timestamp,
    ProgressItem Day,
    ProgressItem Month,
    ProgressItem Year,
    ProgressItem? Life);
