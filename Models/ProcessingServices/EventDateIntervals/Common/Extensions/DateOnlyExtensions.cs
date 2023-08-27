
namespace Models.ProcessingServices.EventDateIntervals.Common.Extensions;

public static class DateOnlyExtensions
{
    public static DateTime NearestDayOfWeek(this DateTime date, DayOfWeek desiredDay)
       => desiredDay == date.DayOfWeek
           ? date
           : date.AddDays((desiredDay - date.DayOfWeek + 7) % 7);
}
