using Playbill.Common;
using Playbill.Services.EventDateIntervals.Common.Extension;
using Playbill.Services.EventDateIntervals.Common.Service;

namespace Playbill.Services.EventDateIntervals;

internal class EventDateIntervalsService : BaseEventDateIntervalsService
{
    public override IList<EventDateInterval> GetByRange(DateOnly startDate, DateOnly endDate)
    {
        return new List<EventDateInterval>(){ new EventDateInterval()
        {
            StartDate = startDate,
            EndDate = endDate
        }};
    }

    public override IList<EventDateInterval> GetMonth()
    {
        var now = DateTime.Now;
        return new List<EventDateInterval>(){ new EventDateInterval()
        {
            StartDate = DateOnly.FromDateTime(now),
            EndDate = new DateOnly(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month)),
        }};
    }

    public override IList<EventDateInterval> GetNextWeekend()
    {
        var now = DateTime.Now;

        var startDate = DateOnly.FromDateTime(now);

        if (now.DayOfWeek == DayOfWeek.Sunday)
        {

            return new List<EventDateInterval>(){ new EventDateInterval()
            {
                StartDate = startDate,
                EndDate = startDate,
            }};
        }

        startDate = startDate.NearestDayOfWeek(DayOfWeek.Saturday);
        return new List<EventDateInterval>(){ new EventDateInterval()
        {
            StartDate = startDate,
            EndDate = startDate.AddDays(1),
        }};
    }

    public override IList<EventDateInterval> GetNext30Days()
    {
        var now = DateTime.Now;
        return new List<EventDateInterval>(){ new EventDateInterval()
        {
            StartDate = DateOnly.FromDateTime(now),
            EndDate = DateOnly.FromDateTime(now.AddDays(30)),
        }};
    }

    public override IList<EventDateInterval> GetNext30DaysWeekends()
    {
        var result = new List<EventDateInterval>();

        var nextWeekend = GetNextWeekend().First();
        result.Add(nextWeekend);

        var startDateForSearch = nextWeekend;

        if (startDateForSearch.EndDate == startDateForSearch.StartDate)
        {
            startDateForSearch.StartDate = startDateForSearch.EndDate.AddDays(-1);
        }

        var maxEndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));

        for (var i = 1; i <= 4; i++)//30 days / 7 = 4
        {
            var nextWeekWeekend = startDateForSearch;
            nextWeekWeekend.StartDate = startDateForSearch.StartDate.AddDays(7 * i);
            nextWeekWeekend.EndDate = startDateForSearch.EndDate.AddDays(7 * i);

            if (nextWeekWeekend.StartDate > maxEndDate)
            {
                break;
            }

            if (nextWeekWeekend.StartDate == maxEndDate || nextWeekWeekend.EndDate > maxEndDate)
            {
                nextWeekWeekend.EndDate = nextWeekWeekend.StartDate;
                result.Add(nextWeekWeekend);
                break;
            }
            result.Add(nextWeekWeekend);
        }

        return result;
    }

    public override IList<EventDateInterval> GetNextWeek()
    {
        var now = DateTime.Now;
        var startDate = DateOnly
            .FromDateTime(now)
            .NearestDayOfWeek(DayOfWeek.Monday)
            .AddDays(now.DayOfWeek == DayOfWeek.Monday ? 7 : 0);

        return new List<EventDateInterval>(){ new EventDateInterval()
        {
            StartDate = startDate,
            EndDate = startDate.AddDays(6),
        }};
    }

    public override IList<EventDateInterval> GetNextWeekWeekend()
    {
        var now = DateTime.Now;
        var startDate =  DateOnly.FromDateTime(now)
            .NearestDayOfWeek(DayOfWeek.Saturday)
            .AddDays(now.DayOfWeek == DayOfWeek.Sunday ? 0 : 7);

        return new List<EventDateInterval>(){ new EventDateInterval()
        {
            StartDate = startDate,
            EndDate = startDate.AddDays(1),
        }};
    }

    public override IList<EventDateInterval> GetThisMonthWeekends()
    {
        var result = new List<EventDateInterval>();
        var now = DateTime.Now;
        var thisMonth = now.Month;

        var lastMonthDay = DateTime.DaysInMonth(now.Year, thisMonth);

        var nextWeekend = GetNextWeekend().First();

        if (nextWeekend.StartDate.Month > thisMonth)
        {
            return result;
        }

        if (nextWeekend.EndDate.Month > thisMonth)
        {
            return new List<EventDateInterval>(){ new EventDateInterval()
            {
                StartDate = nextWeekend.StartDate,
                EndDate = nextWeekend.StartDate,
            }};
        }
        result.Add(nextWeekend);

        var startDateForSearch = nextWeekend;

        if (startDateForSearch.EndDate == startDateForSearch.StartDate)
        {
            startDateForSearch.StartDate = startDateForSearch.EndDate.AddDays(-1);
        }

        for (var i = 1; i <= (lastMonthDay - now.Day) / 7; i++)
        {
            var nextWeekWeekend = startDateForSearch;
            nextWeekWeekend.StartDate = startDateForSearch.StartDate.AddDays(7 * i);
            nextWeekWeekend.EndDate = startDateForSearch.EndDate.AddDays(7 * i);
            if (nextWeekWeekend.StartDate.Month > thisMonth)
            {
                break;
            }
            if (nextWeekWeekend.EndDate.Month > thisMonth)
            {
                nextWeekWeekend.EndDate = nextWeekWeekend.StartDate;
                result.Add(nextWeekWeekend);
                break;
            }
            result.Add(nextWeekWeekend);
        }

        return result;
    }

    public override IList<EventDateInterval> GetWeek()
    {
        var now = DateTime.Now;
        var startDate = DateOnly.FromDateTime(now);
        return new List<EventDateInterval>(){ new EventDateInterval()
        {
            StartDate = startDate,
            EndDate = startDate.NearestDayOfWeek(DayOfWeek.Monday).AddDays(now.DayOfWeek == DayOfWeek.Monday ? 6 : -1),
        }};
    }
}
