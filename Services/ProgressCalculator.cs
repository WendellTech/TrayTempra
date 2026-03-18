using TrayTempra.Models;

namespace TrayTempra.Services;

public static class ProgressCalculator
{
    public static ProgressSnapshot BuildSnapshot(DateTime now, AppSettings settings)
    {
        settings.Normalize();

        var day = CalculateDay(now, settings.DayStartHour);
        var month = CalculateMonth(now, settings.MonthStartDay);
        var year = CalculateYear(now, settings.YearStartMonth, settings.YearStartDay);
        ProgressItem? life = settings.ShowLife ? CalculateLife(now, settings.BirthDate, settings.LifespanYears) : null;

        return new ProgressSnapshot(now, day, month, year, life);
    }

    private static ProgressItem CalculateDay(DateTime now, int startHour)
    {
        var anchor = new DateTime(now.Year, now.Month, now.Day, startHour, 0, 0, now.Kind);
        if (now < anchor) anchor = anchor.AddDays(-1);
        var end = anchor.AddDays(1);
        return new ProgressItem("Day", Ratio(now, anchor, end));
    }

    private static ProgressItem CalculateMonth(DateTime now, int startDay)
    {
        var currentStartDay = Math.Min(startDay, DateTime.DaysInMonth(now.Year, now.Month));
        var start = new DateTime(now.Year, now.Month, currentStartDay, 0, 0, 0, now.Kind);
        if (now < start)
        {
            var previousMonth = now.AddMonths(-1);
            var previousStartDay = Math.Min(startDay, DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month));
            start = new DateTime(previousMonth.Year, previousMonth.Month, previousStartDay, 0, 0, 0, now.Kind);
        }

        var next = start.AddMonths(1);
        return new ProgressItem("Month", Ratio(now, start, next));
    }

    private static ProgressItem CalculateYear(DateTime now, int startMonth, int startDay)
    {
        var currentStartDay = Math.Min(startDay, DateTime.DaysInMonth(now.Year, startMonth));
        var start = new DateTime(now.Year, startMonth, currentStartDay, 0, 0, 0, now.Kind);
        if (now < start)
        {
            var previousYear = now.Year - 1;
            var previousStartDay = Math.Min(startDay, DateTime.DaysInMonth(previousYear, startMonth));
            start = new DateTime(previousYear, startMonth, previousStartDay, 0, 0, 0, now.Kind);
        }

        var nextStartDay = Math.Min(startDay, DateTime.DaysInMonth(start.Year + 1, startMonth));
        var next = new DateTime(start.Year + 1, startMonth, nextStartDay, 0, 0, 0, now.Kind);
        return new ProgressItem("Year", Ratio(now, start, next));
    }

    private static ProgressItem CalculateLife(DateTime now, DateOnly birthDate, int lifespanYears)
    {
        var start = birthDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local);
        var maxDay = Math.Min(birthDate.Day, DateTime.DaysInMonth(birthDate.Year + lifespanYears, birthDate.Month));
        var endBirth = new DateOnly(birthDate.Year + lifespanYears, birthDate.Month, maxDay);
        var end = endBirth.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local);

        return new ProgressItem("Life", Ratio(now, start, end));
    }

    private static double Ratio(DateTime value, DateTime start, DateTime end)
    {
        var range = end - start;
        if (range <= TimeSpan.Zero) return 0;
        return (value - start).TotalMilliseconds / range.TotalMilliseconds;
    }
}
