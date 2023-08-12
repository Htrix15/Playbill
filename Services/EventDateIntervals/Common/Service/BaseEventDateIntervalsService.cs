using Playbill.Common;
using Playbill.Services.EventDateIntervals.Common.Enums;
using Playbill.Services.EventDateIntervals.Common.Interfaces;

namespace Playbill.Services.EventDateIntervals.Common.Service;

public abstract class BaseEventDateIntervalsService : IGetEventDateIntervals, IGetPresetEventDateIntervals
{
    private Dictionary<PresetEventDateIntervals, Func<IList<EventDateInterval>>> _presetEventDateIntervalsFuncMapping;

    public BaseEventDateIntervalsService()
    {
        _presetEventDateIntervalsFuncMapping = new Dictionary<PresetEventDateIntervals, Func<IList<EventDateInterval>>>
        {
            { PresetEventDateIntervals.NextWeekend, GetNextWeekend },
            { PresetEventDateIntervals.NextWeekWeekend, GetNextWeekWeekend },
            { PresetEventDateIntervals.ThisMonthWeekends, GetThisMonthWeekends },
            { PresetEventDateIntervals.Next30DaysWeekends, GetNext30DaysWeekends },
            { PresetEventDateIntervals.Week, GetWeek },
            { PresetEventDateIntervals.NextWeek, GetNextWeek },
            { PresetEventDateIntervals.Month, GetMonth },
            { PresetEventDateIntervals.Next30Days, GetNext30Days },
        };
    }

    private IList<EventDateInterval> GetEventDateInterval(PresetEventDateIntervals presetEventDateInterval) => _presetEventDateIntervalsFuncMapping[presetEventDateInterval].Invoke();
    private IList<EventDateInterval> GetEventDateInterval(DateOnly startDate, DateOnly endDate) => GetByRange(startDate, endDate);

    public IList<EventDateInterval> GetEventDateInterval(PresetEventDateIntervals? presetEventDateInterval, DateOnly? startDate, DateOnly? endDate)
    {
        if (presetEventDateInterval.HasValue)
        {
            return GetEventDateInterval(presetEventDateInterval.Value);
        }
        if (startDate.HasValue && endDate.HasValue)
        {
            return GetEventDateInterval(startDate.Value, endDate.Value);
        }
        throw new Exception("Invalid date options");
    }
     
    public abstract IList<EventDateInterval> GetByRange(DateOnly startDate, DateOnly endDate);

    public abstract IList<EventDateInterval> GetMonth();

    public abstract IList<EventDateInterval> GetNext30Days();

    public abstract IList<EventDateInterval> GetNext30DaysWeekends();

    public abstract IList<EventDateInterval> GetNextWeek();

    public abstract IList<EventDateInterval> GetNextWeekend();

    public abstract IList<EventDateInterval> GetNextWeekWeekend();

    public abstract IList<EventDateInterval> GetThisMonthWeekends();

    public abstract IList<EventDateInterval> GetWeek();
}
