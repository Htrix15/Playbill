using Models.ProcessingServices.EventDateIntervals.Common.Enums;

namespace Models.ProcessingServices.EventDateIntervals.Common.Interfaces;

public interface IGetEventDateIntervals
{
    Task<IList<EventDateInterval>> GetDateIntervalsAsync(HashSet<DayOfWeek> daysOfWeek, 
        DatePeriods? datePeriods = null, 
        DateOnly? startDate = null, 
        DateOnly? endDate = null,
        bool addHolidays = false);
}