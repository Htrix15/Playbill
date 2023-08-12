using Playbill.Common;

namespace Playbill.Services.EventDateIntervals.Common.Interfaces;

public interface IGetEventDateIntervals
{
    IList<EventDateInterval> GetByRange(DateOnly startDate, DateOnly endDate);
}