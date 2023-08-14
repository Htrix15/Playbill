using Playbill.Billboards.Common.Event;

namespace Playbill.Billboards.Common.Interfaces;

public interface IConvertToEvent<TEventTypeKey>
{
    (IList<Event.Event> events, IList<Event.Event> failedEvents) ConvertToEvents(BaseConvertToEventSetting setting);
}
