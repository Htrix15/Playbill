using Playbill.Common;
using Playbill.Services.EventDateIntervals.Common.Enums;
using Playbill.Services.EventDateIntervals.Common.Exceptions;
using Playbill.Services.EventDateIntervals.Common.Extensions;
using Playbill.Services.EventDateIntervals.Common.Interfaces;

namespace Playbill.Services.EventDateIntervals;

internal class EventDateIntervalsService : IGetEventDateIntervals
{
    private enum Direction
    {
        Raise,
        Lower
    }

    private DateTime GetNearestDayFromDaysOfWeek(DateTime minDate, 
        DateTime maxDate, 
        HashSet<DayOfWeek> daysOfWeek, 
        Direction direction = Direction.Raise)
    {
        var date = direction switch
        {
            Direction.Raise => minDate,
            Direction.Lower => maxDate,
            _ => throw new NotImplementedException()
        };

        var step = direction switch
        {
            Direction.Raise => 1,
            Direction.Lower => -1,
            _ => throw new NotImplementedException()
        };

        while (!daysOfWeek.Contains(date.DayOfWeek))
        {
            date = date.AddDays(step);
            if (date.Date == maxDate.Date && !daysOfWeek.Contains(date.DayOfWeek))
            {
                throw new OutOfRangeDayException();
            }
        }
        return date;
    }


    public IList<EventDateInterval> GetDateIntervals(HashSet<DayOfWeek> daysOfWeek, DatePeriods? datePeriods = null, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        if ((startDate.HasValue && !endDate.HasValue) || (!startDate.HasValue && endDate.HasValue))
        {
            throw new Exception("Invalid input options. Check: " + (startDate.HasValue ? "endDate" : "startDate"));
        }

        if (!startDate.HasValue && !endDate.HasValue && !datePeriods.HasValue)
        {
            throw new Exception("Invalid input options. Check: datePeriods, endDate, startDate");
        }

        var result = new List<EventDateInterval>();
        if (!daysOfWeek.Any())
        {
            daysOfWeek = new HashSet<DayOfWeek>() { 
                DayOfWeek.Sunday,
                DayOfWeek.Monday,
                DayOfWeek.Tuesday,
                DayOfWeek.Wednesday,
                DayOfWeek.Thursday,
                DayOfWeek.Friday,
                DayOfWeek.Saturday,
            };
        }

        var minDate = DateTime.Now;
        if (startDate.HasValue && startDate > DateOnly.FromDateTime(minDate))
        {
            minDate = startDate.Value.ToDateTime(TimeOnly.MinValue);
        }

        var maxDate = DateTime.Now;
        if (endDate.HasValue && endDate > DateOnly.FromDateTime(maxDate))
        {
            maxDate = endDate.Value.ToDateTime(TimeOnly.MaxValue);
        }

        if (datePeriods.HasValue && !(startDate.HasValue && endDate.HasValue))
        {
            minDate = datePeriods switch
            {
                DatePeriods.ThisWeek => minDate,
                DatePeriods.NextWeek => minDate.NearestDayOfWeek(DayOfWeek.Monday).AddDays(minDate.DayOfWeek == DayOfWeek.Monday ? 7 : 0),
                DatePeriods.ThisMonth => minDate,
                DatePeriods.Next30Days => minDate,
                DatePeriods.ThisYear => minDate,
                _ => throw new NotImplementedException(),
            };

            maxDate = datePeriods switch
            {
                DatePeriods.ThisWeek => minDate.NearestDayOfWeek(DayOfWeek.Monday).AddDays(minDate.DayOfWeek == DayOfWeek.Monday ? 6 : -1),
                DatePeriods.NextWeek => minDate.AddDays(6),
                DatePeriods.ThisMonth => new DateTime(minDate.Year, minDate.Month, DateTime.DaysInMonth(minDate.Year, minDate.Month)),
                DatePeriods.Next30Days => minDate.AddDays(30),
                DatePeriods.ThisYear => new DateTime(minDate.Year, 12, 31),
                _ => throw new NotImplementedException(),
            };
        }

        if (daysOfWeek.Count == 7)
        {
            result.Add(new EventDateInterval()
            {
                StartDate = DateOnly.FromDateTime(minDate),
                EndDate = DateOnly.FromDateTime(maxDate)
            });
            return result;
        }

        if (minDate.Date == maxDate.Date) {

            if (daysOfWeek.Contains(minDate.DayOfWeek))
            {
                result.Add(new EventDateInterval()
                {
                    StartDate = DateOnly.FromDateTime(minDate),
                    EndDate = DateOnly.FromDateTime(minDate)
                });
            }
            else
            {
                return result;
            }
        }

        try
        {
            minDate = GetNearestDayFromDaysOfWeek(minDate, maxDate, daysOfWeek);
            maxDate = GetNearestDayFromDaysOfWeek(minDate, maxDate, daysOfWeek, Direction.Lower);
        }
        catch(OutOfRangeDayException)
        {
            return result;
        }

        var allDate = new List<EventDateInterval>()
        {
            new EventDateInterval() {
                StartDate = DateOnly.FromDateTime(minDate),
                EndDate = DateOnly.FromDateTime(minDate)
            }
        };

        var searchDay = minDate.AddDays(1);
        while (searchDay.Date <= maxDate.Date)
        {
            searchDay = GetNearestDayFromDaysOfWeek(searchDay, maxDate, daysOfWeek);
            allDate.Add(new EventDateInterval()
            {
                StartDate = DateOnly.FromDateTime(searchDay),
                EndDate = DateOnly.FromDateTime(searchDay)
            });
            searchDay = searchDay.AddDays(1);
        }

        if (allDate.Count > 1)
        {
            var i = 0;
            do
            {
                var newStartDate = allDate[i].StartDate;
                var newEndDate = newStartDate;
                var potencialEndDates = allDate.Skip(i + 1).ToList();
                potencialEndDates.ForEach(potencialEndDate =>
                {
                    if ((potencialEndDate.StartDate.ToDateTime(TimeOnly.MaxValue) - newEndDate.ToDateTime(TimeOnly.MaxValue)).TotalDays == 1)
                    {
                        newEndDate = potencialEndDate.EndDate;
                        allDate.Remove(potencialEndDate);
                    }
                });
                result.Add(new EventDateInterval()
                {
                    StartDate = newStartDate,
                    EndDate = newEndDate
                });
                i++;
            } while (i < allDate.Count);
        } 
        else
        {
            return allDate;
        }

        return result;
    }


}
