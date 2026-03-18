namespace TrayTempra.Models;

public sealed class AppSettings
{
    public string ThemeId { get; set; } = "PlainDark";
    public bool ShowDay { get; set; } = true;
    public bool ShowMonth { get; set; } = true;
    public bool ShowYear { get; set; } = true;
    public bool ShowLife { get; set; }
    public DateOnly BirthDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(-30));
    public int LifespanYears { get; set; } = 85;
    public int DayStartHour { get; set; }
    public int MonthStartDay { get; set; } = 1;
    public int YearStartMonth { get; set; } = 1;
    public int YearStartDay { get; set; } = 1;
    public bool StartWithWindows { get; set; }

    public AppSettings Normalize()
    {
        ThemeId = string.IsNullOrWhiteSpace(ThemeId) ? "PlainDark" : ThemeId.Trim();
        LifespanYears = Math.Clamp(LifespanYears, 1, 150);
        DayStartHour = Math.Clamp(DayStartHour, 0, 23);
        MonthStartDay = Math.Clamp(MonthStartDay, 1, 28);
        YearStartMonth = Math.Clamp(YearStartMonth, 1, 12);
        YearStartDay = Math.Clamp(YearStartDay, 1, 28);

        var minBirth = new DateOnly(1900, 1, 1);
        var maxBirth = DateOnly.FromDateTime(DateTime.Today);
        if (BirthDate < minBirth) BirthDate = minBirth;
        if (BirthDate > maxBirth) BirthDate = maxBirth;

        return this;
    }

    public AppSettings Clone() => new()
    {
        ThemeId = ThemeId,
        ShowDay = ShowDay,
        ShowMonth = ShowMonth,
        ShowYear = ShowYear,
        ShowLife = ShowLife,
        BirthDate = BirthDate,
        LifespanYears = LifespanYears,
        DayStartHour = DayStartHour,
        MonthStartDay = MonthStartDay,
        YearStartMonth = YearStartMonth,
        YearStartDay = YearStartDay,
        StartWithWindows = StartWithWindows
    };
}
