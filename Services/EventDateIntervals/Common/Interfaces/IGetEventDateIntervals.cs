using Playbill.Common;
using Playbill.Services.EventDateIntervals.Common.Enums;

namespace Playbill.Services.EventDateIntervals.Common.Interfaces;

public interface IGetEventDateIntervals
{
    Task<IList<EventDateInterval>> GetDateIntervalsAsync(HashSet<DayOfWeek> daysOfWeek, 
        DatePeriods? datePeriods = null, 
        DateOnly? startDate = null, 
        DateOnly? endDate = null,
        bool addHolidays = false);
}