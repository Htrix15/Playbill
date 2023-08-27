using Models.Events;

namespace Models.Billboards.Common.Interfaces;

public interface IConvertToEvent<TEventTypeKey>
{
    IList<Event> ConvertToEvents(BaseConvertToEventSetting setting);
}
