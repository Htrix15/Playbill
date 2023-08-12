
namespace Playbill.Services.EventDateIntervals.Common.Extension;

public static class DateOnlyExtensions
{
    public static DateOnly NearestDayOfWeek(this DateOnly date, DayOfWeek desiredDay) 
        => desiredDay == date.DayOfWeek 
            ? date
            : date.AddDays((desiredDay - date.DayOfWeek + 7) % 7);
}
