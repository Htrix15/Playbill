using Playbill.Billboards.Common.Event;

namespace Playbill.Billboards.Common.Interfaces;

public interface IConvertToEvent<TEventTypeKey>
{
    IList<Event.Event> ConvertToEvents(BaseConvertToEventSetting setting);
}
