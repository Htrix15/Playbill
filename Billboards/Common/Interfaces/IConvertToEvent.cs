using Playbill.Billboards.Common.Enums;

namespace Playbill.Billboards.Common.Interfaces;

public interface IConvertToEvent<TEventTypeKey>
{
    (IList<Event.Event> events, IList<Event.Event> failedEvents) ConvertToEvents(Dictionary<TEventTypeKey, EventTypes> eventTypes,
        string? basePathForLink = null,
        int timeOffset = 0);
}
