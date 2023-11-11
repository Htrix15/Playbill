using Microsoft.Extensions.Options;
using Models.ProcessingServices.EventDateIntervals.Common;
using Models.ProcessingServices.EventDateIntervals.Common.Enums;
using Models.ProcessingServices.EventDateIntervals.Common.Exceptions;
using Models.ProcessingServices.EventDateIntervals.Common.Extensions;
using Models.ProcessingServices.EventDateIntervals.Common.Interfaces;
using Models.ProcessingServices.EventDateIntervals.Common.Options;
using System.Xml.Serialization;

namespace Models.ProcessingServices.EventDateIntervals;

public class EventDateIntervalsService : IGetEventDateIntervals
{
    private readonly EventDateIntervalsOptions _eventDateIntervalsOptions;
    public EventDateIntervalsService(IOptions<EventDateIntervalsOptions> eventDateIntervalsOptions)
    {
        _eventDateIntervalsOptions = eventDateIntervalsOptions.Value;
    }

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


    private async Task <List<EventDateInterval>> GetHolidaysIntervalsAsync(DateOnly minDate, DateOnly maxDate)
    {
        var holidays = new List<DateOnly>();
        try
        {
            var httpClient = new HttpClient();
            using var response = await httpClient.GetAsync(_eventDateIntervalsOptions.Request);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            var serializer = new XmlSerializer(typeof(HolidaysResponse));
            HolidaysResponse holidaysResponse;
            using var reader = new StringReader(responseContent);
            holidaysResponse = (HolidaysResponse)serializer.Deserialize(reader);
            holidays = holidaysResponse.OnlyHolidaysDays();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Fail call: {_eventDateIntervalsOptions.Request} Message : {exception.Message}");
            holidays = _eventDateIntervalsOptions.MainHolidays.ToList();
        }

        return holidays
            .Where(holiday => holiday >= minDate && holiday <= maxDate)
            .Select(holiday => new EventDateInterval()
            {
                StartDate = holiday,
                EndDate = holiday
            })
            .ToList();
    }

    public async Task<IList<EventDateInterval>> GetDateIntervalsAsync(HashSet<DayOfWeek> daysOfWeek, 
        DatePeriods? datePeriod = null, 
        DateOnly? startDate = null, 
        DateOnly? endDate = null,
        bool addHolidays = false)
    {
        if ((startDate.HasValue && !endDate.HasValue) || (!startDate.HasValue && endDate.HasValue))
        {
            throw new Exception("Invalid input options. Check: " + (startDate.HasValue ? "endDate" : "startDate"));
        }

        if (!startDate.HasValue && !endDate.HasValue && !datePeriod.HasValue)
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

        if (datePeriod.HasValue && !(startDate.HasValue && endDate.HasValue))
        {
            minDate = datePeriod switch
            {
                DatePeriods.ThisWeek => minDate,
                DatePeriods.NextWeek => minDate.NearestDayOfWeek(DayOfWeek.Monday).AddDays(minDate.DayOfWeek == DayOfWeek.Monday ? 7 : 0),
                DatePeriods.ThisAndNextWeek => minDate,
                DatePeriods.ThisMonth => minDate,
                DatePeriods.Next30Days => minDate,
                DatePeriods.Next60Days => minDate,
                DatePeriods.ThisYear => minDate,
                _ => throw new NotImplementedException(),
            };

            maxDate = datePeriod switch
            {
                DatePeriods.ThisWeek => minDate.NearestDayOfWeek(DayOfWeek.Monday).AddDays(minDate.DayOfWeek == DayOfWeek.Monday ? 6 : -1),
                DatePeriods.NextWeek => minDate.AddDays(6),
                DatePeriods.ThisAndNextWeek => minDate.AddDays(13),
                DatePeriods.ThisMonth => new DateTime(minDate.Year, minDate.Month, DateTime.DaysInMonth(minDate.Year, minDate.Month)),
                DatePeriods.Next30Days => minDate.AddDays(30),
                DatePeriods.Next60Days => minDate.AddDays(60),
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

        if (addHolidays)
        {
            allDate.AddRange(await GetHolidaysIntervalsAsync(DateOnly.FromDateTime(minDate), DateOnly.FromDateTime(maxDate)));
            allDate = allDate.Distinct().OrderBy(date => date.StartDate).ToList();
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
