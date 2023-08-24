using Playbill.Common;
using Playbill.Services.EventDateIntervals.Common.Enums;

namespace Playbill.Services.EventDateIntervals.Common.Interfaces;

public interface IGetEventDateIntervals
{
    IList<EventDateInterval> GetDateIntervals(HashSet<DayOfWeek> daysOfWeek, DatePeriods? datePeriods = null, DateOnly? startDate = null, DateOnly? endDate = null);
}