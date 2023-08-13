using Microsoft.Extensions.Options;
using Playbill.Billboards.Common.Enums;
using Playbill.Billboards.Common.Interfaces;
using Playbill.Billboards.Common.Options;
using Playbill.Common;

namespace Playbill.Billboards.Common.Service;

public abstract class BaseBillboardService: IBillboardService
{
    public abstract BillboardTypes BillboardType { get; }
    protected readonly BaseOptions _options;

    protected BaseBillboardService(IOptions<BaseOptions> options)
    {
        _options = options.Value;
    }
    public abstract Task<(IList<Event.Event>, IList<Event.Event>)> GetEventsAsync(IList<EventDateInterval> eventDateIntervals, IList<EventTypes>? searchEventTypes = null);

    protected IList<Event.Event> FilterEvents(IList<Event.Event> events) =>
        events.Where(@event =>
        {
            var validPlace = !(_options.ExcludePlacesNames?.Contains(@event.Place, StringComparer.CurrentCultureIgnoreCase) ?? true);
            var validTime = @event.Date > DateTime.Now;
            var validName = true;
            if (_options.ExcludeEventsNames?.TryGetValue(@event.Type, out var excludeNames) ?? false)
            {
                validName = excludeNames.Any(name => @event.Title.Contains(name));
            }
            return validPlace && validName && validTime;
        }).ToList();
}
