using Playbill.Common.Event;

namespace Playbill.Billboards.Common.Interfaces;

public interface IConvertToEvent<TEventTypeKey>
{
    IList<Playbill.Common.Event.Event> ConvertToEvents(BaseConvertToEventSetting setting);
}
