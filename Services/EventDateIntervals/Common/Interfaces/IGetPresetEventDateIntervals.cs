using Playbill.Common;

namespace Playbill.Services.EventDateIntervals.Common.Interfaces;

interface IGetPresetEventDateIntervals
{
    IList<EventDateInterval> GetNextWeekend();
    IList<EventDateInterval> GetNextWeekWeekend();
    IList<EventDateInterval> GetThisMonthWeekends();
    IList<EventDateInterval> GetNext30DaysWeekends();
    IList<EventDateInterval> GetWeek();
    IList<EventDateInterval> GetNextWeek();
    IList<EventDateInterval> GetMonth();
    IList<EventDateInterval> GetNext30Days();
}
