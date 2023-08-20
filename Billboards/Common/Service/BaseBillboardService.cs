using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Billboards.Common.Options;
using Playbill.Common;
using Playbill.Common.Event;

namespace Playbill.Billboards.Common.Service;

public abstract class BaseBillboardService: IBillboardService
{
    public abstract BillboardTypes BillboardType { get; }
    protected readonly BaseOptions _options;

    protected BaseBillboardService(IOptions<BaseOptions> options)
    {
        _options = options.Value;
    }
    public abstract Task<IList<Playbill.Common.Event.Event>> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, IList<EventTypes>? searchEventTypes = null);

    protected IList<Playbill.Common.Event.Event> FilterEvents(IList<Playbill.Common.Event.Event> events) =>
        events.Where(@event =>
        {
            var validPlace = (!(_options.ExcludePlacesNames?.Contains(@event.Place, StringComparer.CurrentCultureIgnoreCase)) ?? true);
            var now = DateTime.Now;
            var nowDateOnly = new DateOnly(now.Year, now.Month, now.Day);
            var validTime = (@event.Dates is null && @event.EstimatedDates is not null)
                || (@event.Dates is not null && @event.Dates.Any(date => date > now))
                || (@event.EstimatedDates is not null && @event.EstimatedDates.Any(date => date > nowDateOnly));
            var validName = true;
            if (_options.ExcludeEventsNames?.TryGetValue(@event.Type, out var excludeNames) ?? false)
            {
                validName = excludeNames.Any(name => @event.Title?.Contains(name) ?? true);
            }
            return validPlace && validName && validTime;
        }).ToList();
}
